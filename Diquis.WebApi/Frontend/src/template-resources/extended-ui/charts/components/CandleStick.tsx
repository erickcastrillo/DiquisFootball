import React, { useState } from 'react';
import ReactApexChart from 'react-apexcharts';

import { seriesData } from './data';

type ChartData = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  options: ApexCharts.ApexOptions;
};

const CandleStickChart = () => {
  const colors = ['#47ad77', '#fa5c7c'];
  const [chartData] = useState<ChartData>({
    series: [
      {
        data: seriesData
      }
    ],
    options: {
      chart: {
        height: 400,
        type: 'candlestick'
      },
      plotOptions: {
        candlestick: {
          colors: {
            upward: colors[0],
            downward: colors[1]
          }
        }
      },
      stroke: {
        show: true,
        colors: ['#f1f3fa'],
        width: 1
      },
      xaxis: {
        type: 'datetime'
      },
      grid: {
        borderColor: '#f1f3fa'
      }
    }
  });

  return (
    <div>
      <div id="candlestick-chart">
        <ReactApexChart
          options={chartData.options}
          series={chartData.series}
          type="candlestick"
          height={380}
        />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default CandleStickChart;
