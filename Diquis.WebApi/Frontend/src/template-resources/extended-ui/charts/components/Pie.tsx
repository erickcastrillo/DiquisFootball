import React, { useState } from 'react';
import ReactApexChart from 'react-apexcharts';

type ChartData = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  options: ApexCharts.ApexOptions;
};

const PieChart = () => {
  const [chartData] = useState<ChartData>({
    series: [44, 55, 41, 17, 15],
    options: {
      chart: {
        height: 320,
        type: 'pie'
      },

      labels: ['Series 1', 'Series 2', 'Series 3', 'Series 4', 'Series 5'],
      colors: ['#727cf5', '#6c757d', '#0acf97', '#fa5c7c', '#e3eaef'],
      legend: {
        show: true,
        position: 'bottom',
        horizontalAlign: 'center',
        verticalAlign: 'middle',
        floating: false,
        fontSize: '14px',
        offsetX: 0,
        offsetY: 7
      } as any,
      responsive: [
        {
          breakpoint: 600,
          options: {
            chart: {
              height: 240
            },
            legend: {
              show: false
            }
          }
        }
      ]
    }
  });

  return (
    <div>
      <div id="pie-chart">
        <ReactApexChart
          options={chartData.options}
          series={chartData.series}
          type="pie"
          height={320}
        />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default PieChart;
