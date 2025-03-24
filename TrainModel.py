import torch
from transformers import AutoTokenizer, AutoModelForSequenceClassification, TrainingArguments, Trainer
from datasets import Dataset, DatasetDict
from sklearn.metrics import accuracy_score, f1_score
import gradio as gr

# Step 1: Prepare the Dataset
data = {
    "train": {
        "email": [
            "Please send me the latest sales report.",
            "Your order has been shipped.",
            "Meeting rescheduled to 3 PM tomorrow.",
            "Your password has been successfully changed.",
            "Invoice #567 has been paid successfully.",
            "Your shipment tracking number is 12345."
        ],
        "category": ["business", "shipping", "calendar", "account", "business", "shipping"]
    },
    "test": {
        "email": [
            "Can you confirm the payment receipt?",
            "Order #12345 has been delivered.",
            "Let's schedule a team meeting for next week."
        ],
        "category": ["business", "shipping", "calendar"]
    }
}

# Map labels
label_map = {"business": 0, "shipping": 1, "calendar": 2, "account": 3}
num_labels = len(label_map)

# Convert to DatasetDict
train_data = {
    "email": data["train"]["email"],
    "label": [label_map[cat] for cat in data["train"]["category"]]
}
test_data = {
    "email": data["test"]["email"],
    "label": [label_map[cat] for cat in data["test"]["category"]]
}

datasets = DatasetDict({
    "train": Dataset.from_dict(train_data),
    "test": Dataset.from_dict(test_data)
})

# Step 2: Tokenize the Dataset
tokenizer = AutoTokenizer.from_pretrained("bert-base-uncased")

def tokenize_function(examples):
    return tokenizer(examples["email"], padding="max_length", truncation=True)

tokenized_datasets = datasets.map(tokenize_function, batched=True, remove_columns=["email"])

# Step 3: Load Pre-trained Model
model = AutoModelForSequenceClassification.from_pretrained("bert-base-uncased", num_labels=num_labels)

# Step 4: Define Metrics for Evaluation
def compute_metrics(pred):
    logits, labels = pred
    predictions = torch.argmax(torch.tensor(logits), dim=-1)
    accuracy = accuracy_score(labels, predictions)
    f1 = f1_score(labels, predictions, average="weighted")
    return {"accuracy": accuracy, "f1": f1}

# Step 5: Training Arguments
training_args = TrainingArguments(
    output_dir="./results",
    evaluation_strategy="epoch",
    learning_rate=2e-5,
    per_device_train_batch_size=8,
    per_device_eval_batch_size=8,
    num_train_epochs=3,
    weight_decay=0.01,
    logging_dir="./logs",
    save_strategy="epoch",
    load_best_model_at_end=True
)

# Step 6: Trainer API
trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=tokenized_datasets["train"],
    eval_dataset=tokenized_datasets["test"],
    tokenizer=tokenizer,
    compute_metrics=compute_metrics
)

# Step 7: Fine-tune the Model
trainer.train()

# Save the fine-tuned model and tokenizer
model.save_pretrained("./email-classifier")
tokenizer.save_pretrained("./email-classifier")

# Step 8: Gradio Interface for Testing
# Load the saved model for inference
model = AutoModelForSequenceClassification.from_pretrained("./email-classifier")
tokenizer = AutoTokenizer.from_pretrained("./email-classifier")

# Prediction function
def predict_email_category(email):
    inputs = tokenizer(email, padding=True, truncation=True, return_tensors="pt")
    outputs = model(**inputs)
    logits = outputs.logits
    predicted_class = torch.argmax(logits, dim=-1).item()
    return list(label_map.keys())[predicted_class]

# Gradio UI
with gr.Blocks() as demo:
    gr.Markdown("# Email Category Classifier")
    gr.Markdown("Enter an email text to predict its category.")
    
    email_input = gr.Textbox(label="Email Text")
    output_label = gr.Label(label="Predicted Category")
    
    submit_button = gr.Button("Classify Email")
    submit_button.click(predict_email_category, inputs=email_input, outputs=output_label)

# Run the app
if __name__ == "__main__":
    demo.launch()