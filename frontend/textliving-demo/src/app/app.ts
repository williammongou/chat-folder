import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { provideHttpClient } from '@angular/common/http';
import { ConversationService } from './services/conversation.service';
import { Message, AnalysisResponse } from './models/message.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  conversationText = '';
  analysisResult: AnalysisResponse | null = null;
  isLoading = false;
  errorMessage = '';
  copiedMessageIndex: number | null = null;

  // Sample conversation threads
  sampleConversations = [
    {
      name: 'High Conversion - Ready to Move',
      text: `Property Manager: Hi Sarah! Thanks for your interest in Lakeside Apartments. I'd love to help you find your perfect home. When are you looking to move in?
Prospect: Hi! I'm actually looking to move in by the end of next month. My lease is up on the 30th.
Property Manager: Perfect timing! We have several 2-bedroom units available for move-in on the 1st. What's most important to you in your new apartment?
Prospect: I really need a washer/dryer in unit, and a pet-friendly place for my dog. Also hoping to stay under $1,800/month if possible.
Property Manager: Great news! Our 2BR Unit 304 has in-unit W/D, we're very pet-friendly (we even have a dog park!), and it's $1,750/month. Would you like to schedule a tour this week?
Prospect: That sounds perfect! Yes, I'd love to see it. I'm free Thursday afternoon or Saturday morning.
Property Manager: Excellent! I have Thursday at 3pm available. Does that work?
Prospect: Thursday at 3pm works great. Should I bring anything?
Property Manager: Just your ID and we'll be all set. I'll send you the address and parking info. Looking forward to meeting you Thursday!
Prospect: Perfect, see you then!`
    },
    {
      name: 'Medium Conversion - Price Sensitive',
      text: `Property Manager: Hello Marcus! Thanks for reaching out about our studio apartments. What brings you to our community?
Prospect: Hi, I'm just starting to look around. I saw you have studios listed at $1,200?
Property Manager: Yes! Our studio apartments start at $1,200 for a 450 sq ft unit. They include all utilities except electric. Are you currently living in the area?
Prospect: I'm actually relocating for work. That price seems a bit high for a studio though. Do you have any move-in specials?
Property Manager: I understand budget is important! We're currently offering $500 off first month's rent for new residents. That brings your first month to $700. We also have upgraded studios at 550 sq ft for $1,350 if you'd like more space.
Prospect: Hmm, the $500 off helps. What's included in the building? Any amenities?
Property Manager: Absolutely! We have a fitness center, pool, package lockers, and covered parking. All units have updated kitchens and walk-in closets. When are you planning to move?
Prospect: Probably in the next 2-3 months. I'm still comparing a few places.
Property Manager: That's smart to shop around! Would you like to schedule a tour to see our studios in person? It really helps to see the quality and feel of the space.
Prospect: Maybe. Can I think about it and get back to you?
Property Manager: Of course! No pressure. I'll email you some photos and a virtual tour link. Feel free to reach out anytime with questions!`
    },
    {
      name: 'Low Conversion - Just Browsing',
      text: `Property Manager: Hi! Thanks for your inquiry about Riverside Commons. How can I help you today?
Prospect: Just looking at options in the area.
Property Manager: Great! What type of apartment are you interested in? We have 1, 2, and 3 bedroom options available.
Prospect: Not sure yet. What are your prices?
Property Manager: Our 1-bedrooms range from $1,400-$1,600, 2-bedrooms are $1,800-$2,100, and 3-bedrooms start at $2,400. All include water and trash. When are you looking to move?
Prospect: Don't have a specific date. Just seeing what's out there.
Property Manager: I understand! Are you currently renting or looking to move to the area?
Prospect: Currently renting.
Property Manager: Got it! Well, if you'd like to see our community, I'd be happy to schedule a tour at your convenience. We have some beautiful units available.
Prospect: Ok thanks.
Property Manager: You're welcome! Is there anything specific you're looking for in your next apartment that I can help with?
Prospect: Not really, just browsing for now.`
    }
  ];

  constructor(private conversationService: ConversationService) {}

  loadSample(index: number): void {
    this.conversationText = this.sampleConversations[index].text;
    this.analysisResult = null;
    this.errorMessage = '';
  }

  analyzeConversation(): void {
    if (!this.conversationText.trim()) {
      this.errorMessage = 'Please enter a conversation to analyze';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.analysisResult = null;

    const messages = this.parseConversation(this.conversationText);

    if (messages.length < 2) {
      this.errorMessage = 'Please provide a conversation with at least 2 messages';
      this.isLoading = false;
      return;
    }

    this.conversationService.analyzeConversation({ messages }).subscribe({
      next: (result) => {
        this.analysisResult = result;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Analysis error:', error);
        this.errorMessage = error.error?.message || 'Failed to analyze conversation. Please check your API connection.';
        this.isLoading = false;
      }
    });
  }

  parseConversation(text: string): Message[] {
    const lines = text.split('\n').filter(line => line.trim());
    const messages: Message[] = [];

    for (const line of lines) {
      // Match pattern: "Sender: Message text"
      const match = line.match(/^([^:]+):\s*(.+)$/);
      if (match) {
        messages.push({
          sender: match[1].trim(),
          text: match[2].trim(),
          timestamp: new Date()
        });
      }
    }

    return messages;
  }

  getSentimentClass(sentiment: string): string {
    if (sentiment === 'Positive') return 'sentiment-positive';
    if (sentiment === 'Negative') return 'sentiment-negative';
    return 'sentiment-neutral';
  }

  getUrgencyClass(urgency: string): string {
    if (urgency === 'High') return 'urgency-high';
    if (urgency === 'Medium') return 'urgency-medium';
    return 'urgency-low';
  }

  getConversionClass(score: number): string {
    if (score >= 70) return 'conversion-high';
    if (score >= 40) return 'conversion-medium';
    return 'conversion-low';
  }

  copyToClipboard(message: string, index: number): void {
    navigator.clipboard.writeText(message).then(() => {
      this.copiedMessageIndex = index;
      setTimeout(() => {
        this.copiedMessageIndex = null;
      }, 2000);
    });
  }

  clearForm(): void {
    this.conversationText = '';
    this.analysisResult = null;
    this.errorMessage = '';
  }
}
