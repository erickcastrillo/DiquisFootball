import useToggle from './useToggle';

const useModal = () => {
  const [isOpen, _, show, hide] = useToggle();

  return {
    show: isOpen,
    onShow: show,
    onHide: hide
  };
};

export default useModal;
