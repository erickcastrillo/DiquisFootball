import { useEffect } from 'react';

export default function useAccountLayout() {
  useEffect(() => {
    if (document.body) {
      document.body.classList.add('authentication-bg');
      document.body.classList.add('pb-0');
    }

    return () => {
      if (document.body) document.body.classList.remove('authentication-bg');
      document.body.classList.remove('pb-0');
    };
  }, []);
}
