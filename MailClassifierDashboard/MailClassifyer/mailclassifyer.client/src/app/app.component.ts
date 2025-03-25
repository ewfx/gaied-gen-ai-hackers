import { HttpClient } from '@angular/common/http';
import {  Component, ComponentRef, ViewContainerRef,OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BehaviorSubject } from 'rxjs';
import { compileComponentFromMetadata } from '@angular/compiler';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface Email {  
  mailFrom: string;
  mailTo: string;
  mailSubject: string;
  mailText: string;
  mailTimestamp: string;
  businessCategory: string;
  severity: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];
  public emails: Email[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit() {
    //this.getForecasts();
    this.getEmails();
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('/weatherforecast').subscribe(
      (result) => {
        this.forecasts = result;
        console.log(result);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  getEmails() {
    this.http.get<Email[]>('/emailclassifyer').subscribe(
      (result) => {
        this.emails = result;      
        console.log(result);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  title = 'mailclassifyer.client';
}
