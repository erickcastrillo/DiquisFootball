import React, { useCallback, useState } from 'react';
import ReactApexChart from 'react-apexcharts';

type ChartData = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  options: ApexCharts.ApexOptions;
};

const BubbleChart = () => {
  const generateData = useCallback(
    (baseval: number, count: number, yrange: { min: number; max: number }) => {
      var i = 0;
      var series = [];
      while (i < count) {
        var x = Math.floor(Math.random() * (750 - 1 + 1)) + 1;
        var y =
          Math.floor(Math.random() * (yrange.max - yrange.min + 1)) +
          yrange.min;
        var z = Math.floor(Math.random() * (75 - 15 + 1)) + 15;

        series.push([x, y, z]);
        baseval += 86400000;
        i++;
      }
      return series;
    },
    []
  );

  const [chartData] = useState<ChartData>({
    series: [
      {
        name: 'Bubble 1',
        data: generateData(new Date('11 Feb 2017 GMT').getTime(), 20, {
          min: 10,
          max: 60
        })
      },
      {
        name: 'Bubble 2',
        data: generateData(new Date('11 Feb 2017 GMT').getTime(), 20, {
          min: 10,
          max: 60
        })
      },
      {
        name: 'Bubble 3',
        data: generateData(new Date('11 Feb 2017 GMT').getTime(), 20, {
          min: 10,
          max: 60
        })
      }
    ],
    options: {
      chart: {
        height: 380,
        type: 'bubble',
        toolbar: {
          show: false
        }
      },
      dataLabels: {
        enabled: false
      },
      fill: {
        opacity: 0.8,
        gradient: {
          enabled: false
        } as any
      },
      colors: ['#727cf5', '#ffbc00', '#fa5c7c'],
      xaxis: {
        tickAmount: 12,
        type: 'category'
      },
      yaxis: {
        max: 70
      },
      grid: {
        borderColor: '#f1f3fa',
        padding: {
          bottom: 5
        }
      },
      legend: {
        offsetY: 7
      }
    }
  });

  return (
    <div>
      <div id="bubble-chart">
        <ReactApexChart
          options={chartData.options}
          series={chartData.series}
          type="bubble"
          height={380}
        />
      </div>
      <div id="html-dist"></div>
    </div>
  );
};

export default BubbleChart;
