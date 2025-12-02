import { Row, Col } from 'react-bootstrap';

import { PageLayout } from 'components';
import SampleWrapper from 'template-resources/SampleWrapper';
import IonRangeSlider from './IonRangeSlider';

const RangeSlider = () => {
  return (
    <PageLayout title="Range Sliders">
      <Row>
        <Col xl={6}>
          {/* Default */}
          <SampleWrapper
            title="Default"
            subText={<> Start with default options </>}
          >
            <IonRangeSlider id="range_01" />
          </SampleWrapper>
          {/* Prefix */}
          <SampleWrapper
            title="Prefix"
            subText={<> Showing grid and adding prefix "$" </>}
          >
            <IonRangeSlider
              id="range_03"
              data-type="double"
              data-grid="true"
              data-min="0"
              data-max="1000"
              data-from="200"
              data-to="800"
              data-prefix="$"
            />
          </SampleWrapper>
          {/* Step */}
          <SampleWrapper
            title="Step"
            subText={<> Increment with specific value only (step) </>}
          >
            <IonRangeSlider
              id="range_05"
              data-type="double"
              data-grid="true"
              data-min="-1000"
              data-max="1000"
              data-from="-500"
              data-to="500"
              data-step="250"
            />
          </SampleWrapper>
          {/* Prettify Numbers */}
          <SampleWrapper
            title="Prettify Numbers"
            subText={<> Prettify enabled. Much better! </>}
          >
            <IonRangeSlider
              id="range_07"
              data-grid="true"
              data-min="1000"
              data-max="1000000"
              data-step="1000"
              data-from="200000"
              data-to="1000"
              data-prettify_enabled="true"
            />
          </SampleWrapper>
          {/* Extra Example */}
          <SampleWrapper
            title="Extra Example"
            subText={
              <> Want to show that max number is not the biggest one? </>
            }
          >
            <IonRangeSlider
              id="range_09"
              data-grid="true"
              data-min="18"
              data-max="70"
              data-prefix="Age"
              data-max_postfix="+"
              data-from="30"
              data-to="1000"
            />
          </SampleWrapper>
          {/* Postfixes */}
          <SampleWrapper title="Postfixes" subText={<> Using postfixes </>}>
            <IonRangeSlider
              id="range_11"
              data-type="single"
              data-grid="true"
              data-min="-90"
              data-max="90"
              data-postfix=" Â°"
              data-from="0"
            />
          </SampleWrapper>
        </Col>
        <Col xl={6}>
          {/* Min-Max */}
          <SampleWrapper
            title="Min-Max"
            subText={<> Set min value, max value and start point </>}
          >
            <IonRangeSlider
              id="range_02"
              data-min="100"
              data-max="1000"
              data-from="550"
            />
          </SampleWrapper>
          {/* Range */}
          <SampleWrapper
            title="Range"
            subText={<> Set up range with negative values </>}
          >
            <IonRangeSlider
              id="range_04"
              data-min="-1000"
              data-max="1000"
              data-from="-500"
              data-to="500"
              data-type="double"
              data-grid="true"
            />
          </SampleWrapper>
          {/* Custom Values */}
          <SampleWrapper
            title="Custom Values"
            subText={<> Using any strings as values </>}
          >
            <IonRangeSlider
              id="range_06"
              data-grid="true"
              data-from="3"
              data-values="Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec"
            />
          </SampleWrapper>
          {/* Disabled */}
          <SampleWrapper
            title="Disabled"
            subText={<> Lock slider by using disable option </>}
          >
            <IonRangeSlider
              id="range_08"
              data-min="100"
              data-max="1000"
              data-from="550"
              data-disable="true"
            />
          </SampleWrapper>
          {/* Use Decorate Both option */}
          <SampleWrapper
            title="Use Decorate Both option"
            subText={<> Use decorate_both option </>}
          >
            <IonRangeSlider
              id="range_10"
              data-type="double"
              data-min="100"
              data-max="200"
              data-from="145"
              data-to="155"
              data-prefix="Weight "
              data-postfix=" million pounds"
              data-decorate_both="true"
            />
          </SampleWrapper>
          {/* Hide */}
          <SampleWrapper
            title="Hide"
            subText={<> Or hide any part you wish </>}
          >
            <IonRangeSlider
              id="range_12"
              data-type="double"
              data-min="1000"
              data-max="2000"
              data-from="1200"
              data-to="1800"
              data-hide_min_max="true"
              data-hide_from_to="true"
              data-grid="true"
            />
          </SampleWrapper>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default RangeSlider;
