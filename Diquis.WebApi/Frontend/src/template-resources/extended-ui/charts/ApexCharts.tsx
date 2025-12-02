import React from 'react';
import { Col, Row } from 'react-bootstrap';

import { PageLayout } from 'components';
import SampleWrapper from 'template-resources/SampleWrapper';
import ColumnChart from './components/Column';
import LineChart from './components/Line';
import BubbleChart from './components/Bubble';
import CandleStickChart from './components/CandleStick';
import AreaChart from './components/Area';
import BarChart from './components/Bar';
import PieChart from './components/Pie';
import RadarChart from './components/Radar';
import RadialChart from './components/Radial';
import PolarChart from './components/Polar';

const ApexCharts = () => {
  return (
    <PageLayout title="Charts">
      <Row>
        {/* Column Chart */}
        <Col xl="6">
          <SampleWrapper title="Column Chart">
            <div dir="ltr">
              <ColumnChart />
            </div>
          </SampleWrapper>
        </Col>
        {/* Line Chart */}
        <Col xl="6">
          <SampleWrapper title="Line Chart">
            <div dir="ltr">
              <LineChart />
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Bubble Chart */}
        <Col xl="6">
          <SampleWrapper title="Bubble Chart">
            <div dir="ltr">
              <BubbleChart />
            </div>
          </SampleWrapper>
        </Col>
        {/* CandleStick Chart */}
        <Col xl="6">
          <SampleWrapper title="CandleStick Chart">
            <div dir="ltr">
              <CandleStickChart />
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Area Chart */}
        <Col xl="6">
          <SampleWrapper title="Area Chart">
            <div dir="ltr">
              <AreaChart />
            </div>
          </SampleWrapper>
        </Col>
        {/* Bar Chart */}
        <Col xl="6">
          <SampleWrapper title="Bar Chart">
            <div dir="ltr">
              <BarChart />
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Pie Chart */}
        <Col xl="6">
          <SampleWrapper title="Pie Chart">
            <div dir="ltr">
              <PieChart />
            </div>
          </SampleWrapper>
        </Col>
        {/* Radar Chart */}
        <Col xl="6">
          <SampleWrapper title="Radar Chart">
            <div dir="ltr">
              <RadarChart />
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Radial Chart */}
        <Col xl="6">
          <SampleWrapper title="Radial Chart">
            <div dir="ltr">
              <RadialChart />
            </div>
          </SampleWrapper>
        </Col>
        {/* Polar Chart */}
        <Col xl="6">
          <SampleWrapper title="Polar Chart">
            <div dir="ltr">
              <PolarChart />
            </div>
          </SampleWrapper>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default ApexCharts;
