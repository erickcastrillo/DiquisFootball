import React, { useEffect } from 'react';
import 'ion-rangeslider/js/ion.rangeSlider.min.js';

type IonRangeSliderProps = React.ComponentPropsWithoutRef<'input'>;

const IonRangeSlider = (props: IonRangeSliderProps) => {
  useEffect(() => {
    $('[data-plugin="range-slider"]').each(function () {
      var opts = $(this).data();
      // @ts-ignore-next-line
      $(this).ionRangeSlider(opts);
    });
  }, []);

  return <input type="text" data-plugin="range-slider" {...props} />;
};

export default IonRangeSlider;
