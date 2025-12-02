import React, { useState } from 'react';
import ReactApexChart from 'react-apexcharts';

type ChartData = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  options: ApexCharts.ApexOptions;
};

const AreaChart = () => {
  const [chartData] = useState<ChartData>({
    series: [
      {
        name: 'STOCK ABC',
        data: [
          8107.85, 8128.0, 8122.9, 8165.5, 8340.7, 8423.7, 8423.5, 8514.3,
          8481.85, 8487.7, 8506.9, 8626.2, 8668.95, 8602.3, 8607.55, 8512.9,
          8496.25, 8600.65, 8881.1, 9340.85
        ]
      }
    ],
    options: {
      chart: {
        height: 380,
        type: 'area',
        zoom: {
          enabled: false
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        width: 3,
        curve: 'straight'
      },
      colors: ['#fa6767'],

      title: {
        text: 'Fundamental Analysis of Stocks',
        align: 'left'
      },
      subtitle: {
        text: 'Price Movements',
        align: 'left'
      },
      labels: [
        '13 Nov 2017',
        '14 Nov 2017',
        '15 Nov 2017',
        '16 Nov 2017',
        '17 Nov 2017',
        '20 Nov 2017',
        '21 Nov 2017',
        '22 Nov 2017',
        '23 Nov 2017',
        '24 Nov 2017',
        '27 Nov 2017',
        '28 Nov 2017',
        '29 Nov 2017',
        '30 Nov 2017',
        '01 Dec 2017',
        '04 Dec 2017',
        '05 Dec 2017',
        '06 Dec 2017',
        '07 Dec 2017',
        '08 Dec 2017'
      ],
      xaxis: {
        type: 'datetime'
      },
      yaxis: {
        opposite: true
      },
      legend: {
        horizontalAlign: 'left'
      },
      grid: {
        borderColor: '#f1f3fa'
      },
      responsive: [
        {
          breakpoint: 600,
          options: {
            chart: {
              toolbar: {
                show: false
              }
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
      <div id="area-chart">
        <ReactApexChart
          options={chartData.options}
          series={chartData.series}
          type="area"
          height={380}
        />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default AreaChart;
