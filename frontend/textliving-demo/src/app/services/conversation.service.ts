import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConversationRequest, AnalysisResponse } from '../models/message.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConversationService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  analyzeConversation(request: ConversationRequest): Observable<AnalysisResponse> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post<AnalysisResponse>(
      `${this.apiUrl}/api/conversation/analyze`,
      request,
      { headers }
    );
  }
}
