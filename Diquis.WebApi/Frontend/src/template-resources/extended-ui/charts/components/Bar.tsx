import React, { useState } from 'react';
import ReactApexChart from 'react-apexcharts';

type ChartData = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  options: ApexCharts.ApexOptions;
};

const BarChart = () => {
  const [chartData] = useState<ChartData>({
    series: [
      {
        data: [400, 430, 448, 470, 540, 580, 690, 1100, 1200, 1380]
      }
    ],
    options: {
      chart: {
        height: 380,
        type: 'bar',
        toolbar: {
          show: false
        }
      },
      plotOptions: {
        bar: {
          horizontal: true
        }
      },
      dataLabels: {
        enabled: false
      },

      colors: ['#39afd1'],
      xaxis: {
        categories: [
          'South Korea',
          'Canada',
          'United Kingdom',
          'Netherlands',
          'Italy',
          'France',
          'Japan',
          'United States',
          'China',
          'Germany'
        ],
        axisBorder: {
          show: false
        }
      },
      states: {
        hover: {
          filter: 'none' as any
        }
      },
      grid: {
        borderColor: '#f1f3fa'
      }
    }
  });

  return (
    <div>
      <div id="bar-chart">
        <ReactApexChart
          options={chartData.options}
          series={chartData.series}
          type="bar"
          height={380}
        />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default BarChart;
