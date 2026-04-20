import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import {
  ChatMessage,
  CampaignGenerationRequest,
  CampaignGenerationResponse,
  TokenUsageResponse,
  HistoricalCampaignData
} from './models/campaign.model';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class AppComponent {
  private apiUrl = environment.apiUrl + '/api/campaign';

  // Chat state
  messages: ChatMessage[] = [];
  userInput: string = '';
  isLoading: boolean = false;
  errorMessage: string | null = null;

  // Token usage
  tokenUsage: TokenUsageResponse = {
    tokensUsedToday: 0,
    tokenLimit: 10000,
    percentageUsed: 0,
    status: 'green'
  };

  // Historical data
  historicalData: HistoricalCampaignData | null = null;
  showIngestButton: boolean = true;

  // File upload
  fileInputElement: HTMLInputElement | null = null;

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {
    this.addWelcomeMessage();
    this.loadTokenUsage();
  }

  private addWelcomeMessage() {
    this.messages.push({
      role: 'assistant',
      content: '👋 Welcome to TextLiving Campaign Generator!\n\nI help you create high-converting promotional SMS messages based on your historical campaign data.\n\n**To get started:**\n1. **Upload past campaign data** (optional) - Click "Ingest Data" button\n2. **Type a prompt** - Example: "Create a promo text for a free appetizer offer at my restaurant"\n3. **Get 2 optimized options** with insights and best send times\n\nTry it now! What kind of promotional message do you want to create?',
      timestamp: new Date()
    });
  }

  async sendMessage() {
    if (!this.userInput.trim() || this.isLoading) return;

    const userMessage: ChatMessage = {
      role: 'user',
      content: this.userInput,
      timestamp: new Date()
    };

    this.messages.push(userMessage);
    const prompt = this.userInput;
    this.userInput = '';
    this.isLoading = true;
    this.errorMessage = null;

    // Force UI update and scroll
    this.cdr.detectChanges();
    setTimeout(() => this.scrollToBottom(), 100);

    try {
      const request: CampaignGenerationRequest = {
        prompt: prompt,
        historicalData: this.historicalData ?? undefined
      };

      const response = await firstValueFrom(
        this.http.post<CampaignGenerationResponse>(`${this.apiUrl}/generate`, request)
      );

      if (response) {
        const assistantMessage: ChatMessage = {
          role: 'assistant',
          content: this.formatCampaignResponse(response),
          campaignResult: response,
          timestamp: new Date()
        };

        this.messages.push(assistantMessage);

        // Update token usage
        this.tokenUsage.tokensUsedToday = response.totalTokensToday;
        this.tokenUsage.percentageUsed = (response.totalTokensToday / 10000) * 100;
        this.tokenUsage.status = this.getTokenStatus(this.tokenUsage.percentageUsed);

        // Force UI update and scroll
        this.cdr.detectChanges();
        setTimeout(() => this.scrollToBottom(), 100);
      }
    } catch (error: any) {
      this.errorMessage = error.error?.error || 'Failed to generate campaign. Please try again.';
      console.error('Error generating campaign:', error);
    } finally {
      this.isLoading = false;
    }
  }

  private formatCampaignResponse(response: CampaignGenerationResponse): string {
    return `Here are 2 optimized promotional SMS options:\n\n**Option 1:**\n"${response.option1.messageText}"\n\n✅ Why this works: ${response.option1.whyThisWorks}\n📊 Best send time: ${response.option1.bestSendTime}\n🎯 Predicted conversion: ${response.option1.predictedConversionRate}%\n💬 Tone: ${response.option1.toneAnalysis}\n\n**Option 2:**\n"${response.option2.messageText}"\n\n✅ Why this works: ${response.option2.whyThisWorks}\n📊 Best send time: ${response.option2.bestSendTime}\n🎯 Predicted conversion: ${response.option2.predictedConversionRate}%\n💬 Tone: ${response.option2.toneAnalysis}`;
  }

  async loadTokenUsage() {
    try {
      const usage = await firstValueFrom(
        this.http.get<TokenUsageResponse>(`${this.apiUrl}/token-usage`)
      );

      if (usage) {
        this.tokenUsage = usage;
      }
    } catch (error) {
      console.error('Error loading token usage:', error);
    }
  }

  getTokenStatus(percentage: number): string {
    if (percentage < 50) return 'green';
    if (percentage < 80) return 'orange';
    return 'red';
  }

  getTokenBarClass(): string {
    return `token-bar-${this.tokenUsage.status}`;
  }

  openFileDialog() {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = '.json';
    input.onchange = (e: any) => {
      const file = e.target.files[0];
      if (file) {
        this.loadHistoricalData(file);
      }
    };
    input.click();
  }

  private loadHistoricalData(file: File) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      try {
        const data = JSON.parse(e.target.result);
        this.historicalData = data;
        this.showIngestButton = false;

        this.messages.push({
          role: 'assistant',
          content: `✅ Historical data loaded successfully!\n\n**Business:** ${data.businessName} (${data.businessType})\n**Campaigns analyzed:** ${data.campaigns?.length || 0}\n**Average conversion rate:** ${data.insights?.averageConversionRate || 'N/A'}%\n\nI'll now use this data to generate better campaign suggestions. What kind of promotional message would you like to create?`,
          timestamp: new Date()
        });

        // Force UI update and scroll
        this.cdr.detectChanges();
        setTimeout(() => this.scrollToBottom(), 100);
      } catch (error) {
        this.errorMessage = 'Failed to parse JSON file. Please check the format.';
        console.error('Error parsing file:', error);
      }
    };
    reader.readAsText(file);
  }

  clearChat() {
    this.messages = [];
    this.addWelcomeMessage();
    this.userInput = '';
    this.errorMessage = null;
  }

  copyToClipboard(text: string) {
    navigator.clipboard.writeText(text).then(() => {
      console.log('Copied to clipboard:', text);
    }).catch(err => {
      console.error('Failed to copy:', err);
    });
  }

  formatMarkdown(text: string): string {
    // Simple markdown formatting
    return text
      .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>') // Bold
      .replace(/\n/g, '<br>'); // Line breaks
  }

  private scrollToBottom(): void {
    const messagesArea = document.querySelector('.messages-area');
    if (messagesArea) {
      messagesArea.scrollTop = messagesArea.scrollHeight;
    }
  }
}
