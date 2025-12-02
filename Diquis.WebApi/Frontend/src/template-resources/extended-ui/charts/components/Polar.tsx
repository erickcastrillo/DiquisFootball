import React, { useState } from 'react';
import ReactApexChart from 'react-apexcharts';

type ChartData = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  options: ApexCharts.ApexOptions;
};

const PolarChart = () => {
  const [chartData] = useState<ChartData>({
    series: [14, 23, 21, 17, 15, 10],
    options: {
      chart: {
        height: 380,
        type: 'polarArea'
      },
      stroke: {
        colors: ['#fff']
      },
      fill: {
        opacity: 0.8
      },
      labels: ['Vote A', 'Vote B', 'Vote C', 'Vote D', 'Vote E', 'Vote F'],
      legend: {
        position: 'bottom'
      },
      colors: [
        '#727cf5',
        '#6c757d',
        '#0acf97',
        '#fa5c7c',
        '#ffbc00',
        '#39afd1'
      ],
      responsive: [
        {
          breakpoint: 480,
          options: {
            chart: {
              width: 200
            },
            legend: {
              position: 'bottom'
            }
          }
        }
      ]
    }
  });

  return (
    <div>
      <div id="polar-chart">
        <ReactApexChart
          options={chartData.options}
          series={chartData.series}
          type="polarArea"
          height={380}
        />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default PolarChart;
