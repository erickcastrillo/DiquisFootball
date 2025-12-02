import React, { useState } from 'react';
import ReactApexChart from 'react-apexcharts';

type ChartData = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  options: ApexCharts.ApexOptions;
};

const RadialChart = () => {
  const [chartData] = useState<ChartData>({
    series: [70],
    options: {
      chart: {
        height: 320,
        type: 'radialBar'
      },
      plotOptions: {
        radialBar: {
          hollow: {
            size: '70%'
          },
          track: {
            background: 'rgba(170,184,197, 0.2)'
          }
        }
      },
      colors: ['#39afd1'],

      labels: ['Acquisition Rate']
    }
  });

  return (
    <div>
      <div id="radial-chart">
        <ReactApexChart
          options={chartData.options}
          series={chartData.series}
          type="radialBar"
          height={320}
        />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default RadialChart;
