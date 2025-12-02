import React, { useState } from 'react';
import ReactApexChart from 'react-apexcharts';

type ChartData = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  options: ApexCharts.ApexOptions;
};

const RadarChart = () => {
  const [chartData] = useState<ChartData>({
    series: [
      {
        name: 'Series 1',
        data: [80, 50, 30, 40, 100, 20]
      }
    ],
    options: {
      chart: {
        height: 350,
        type: 'radar'
      },

      colors: ['#727cf5'],
      labels: ['January', 'February', 'March', 'April', 'May', 'June']
    }
  });

  return (
    <div>
      <div id="radar-chart">
        <ReactApexChart
          options={chartData.options}
          series={chartData.series}
          type="radar"
          height={350}
        />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default RadarChart;
