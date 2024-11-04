import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatMenuModule } from '@angular/material/menu';
import { Router } from '@angular/router';
import { StockResponse, DailyData } from '../../models/stock.interface';
import { StockService } from '../../servicecs/stock.service';
import { AuthService } from '../../servicecs/auth.service';
import { CandleSeriesService, DateTimeService, HiloOpenCloseSeriesService, HiloSeriesService, LineSeriesService, StockChartModule } from '@syncfusion/ej2-angular-charts';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    ReactiveFormsModule,
    CommonModule,
    MatDatepickerModule,
    MatMenuModule,
    StockChartModule,
    MatNativeDateModule
  ],
  providers: [
    CandleSeriesService,
    LineSeriesService,
    HiloSeriesService,
    HiloOpenCloseSeriesService,
    DateTimeService
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  symbols = ['MSFT', 'AAPL', 'NFLX', 'META', 'AMZN'];
  selectedSymbol = 'MSFT';
  selectedTimeframe = '1M';
  chartType: any = 'Candle';
  isLoading = false;
  error: string | null = null;
  stockData?: StockResponse;
  formattedStockData: any[] = [];
  // indicators = ['Moving Average', 'Bollinger Bands', 'MACD']; // indicators

  timeframes = [
    { value: '4H', label: '4 Hours' },
    { value: '1W', label: '1 Week' },
    { value: '1M', label: '1 Month' },
    { value: '3M', label: '3 Months' },
    { value: '6M', label: '6 Months' },
    { value: '1Y', label: '1 Year' },
    { value: 'YTD', label: 'Year to Date' },
    { value: 'All', label: 'All Time' }
  ];

  chartTypes = [
    { value: 'Candle', label: 'Candle Chart' },
    { value: 'Line', label: 'Line Chart' },
    { value: 'Hilo', label: 'Hilo Chart' },
    { value: 'HiloOpenClose', label: 'Hilo Open-Close Chart' }
  ];

  constructor(
    private stockService: StockService,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadStockData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadStockData(): void {
    this.isLoading = true;
    this.error = null;

    this.stockService.getStockData(this.selectedSymbol)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data: StockResponse) => {
          this.stockData = data;
          this.formatStockDataForSyncfusion(data);
          this.isLoading = false;
        },
        error: () => {
          this.error = 'Failed to load stock data. Please try again later.';
          this.snackBar.open(this.error, 'Close', { duration: 5000 });
          this.isLoading = false;
        }
      });
  }

  formatStockDataForSyncfusion(data: StockResponse): void {
    const cutoffDate = this.getTimeframeCutoffDate();
    this.formattedStockData = Object.entries(data.timeSeriesDaily)
      .map(([date, values]: [string, DailyData]) => ({
        x: new Date(date),
        open: parseFloat(values.open),
        high: parseFloat(values.high),
        low: parseFloat(values.low),
        close: parseFloat(values.close),
        volume: parseInt(values.volume, 10)
      }))
      .filter(item => item.x >= cutoffDate)
      .sort((a, b) => a.x.getTime() - b.x.getTime());
  }

  private getTimeframeCutoffDate(): Date {
    const now = new Date();
    switch (this.selectedTimeframe) {
      case '4H': return new Date(now.setDate(now.getHours() - 4));
      case '1W': return new Date(now.setDate(now.getDate() - 7));
      case '1M': return new Date(now.setMonth(now.getMonth() - 1));
      case '3M': return new Date(now.setMonth(now.getMonth() - 3));
      case '6M': return new Date(now.setMonth(now.getMonth() - 6));
      case '1Y': return new Date(now.setFullYear(now.getFullYear() - 1));
      case 'YTD': return new Date(now.getFullYear(), 0, 1);
      default: return new Date(0); // "All" timeframe, show all data
    }
  }

  onSymbolChange(): void {
    this.loadStockData();
  }

  onTimeframeChange(newTimeframe: string): void {
    this.selectedTimeframe = newTimeframe;
    this.loadStockData();
  }

  onChartTypeChange(newType: string): void {
    this.chartType = newType;
  }

  onIndicatorChange(indicator: string): void {
    console.log(`Indicator selected: ${indicator}`);
  }

  retryLoad(): void {
    this.loadStockData();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
