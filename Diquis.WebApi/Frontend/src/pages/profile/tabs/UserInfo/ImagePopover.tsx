import { Dropdown } from 'react-bootstrap';

interface ImagePopoverProps {
  handleImageUpdate: React.ChangeEventHandler<HTMLInputElement>;
  handleImageRemove: () => void;
}

const ImagePopover = (props: ImagePopoverProps) => {
  const handleImageUpdateClick: React.ChangeEventHandler<HTMLInputElement> = (
    evt
  ) => {
    props.handleImageUpdate(evt);
  };
  const handleImageRemoveClick = () => {
    props.handleImageRemove();
  };

  return (
    <div className="profile-avatar__cta">
      <Dropdown autoClose={false}>
        <Dropdown.Toggle
          className="arrow-none rounded-circle btn-sm p-0"
          id="dropdown-basic"
        >
          <i className="mdi mdi-camera font-16" />
        </Dropdown.Toggle>

        <Dropdown.Menu>
          <Dropdown.Item as="div" className="cursor-pointer">
            <label htmlFor="icon-button-file">
              <input
                onChange={handleImageUpdateClick}
                name="imageFile"
                type="file"
                accept="image/*"
                id="icon-button-file"
                style={{ display: 'none' }}
              />
              <span>Upload Picture</span>
            </label>
          </Dropdown.Item>
          {/* <Dropdown.Item
            as="div"
            className="cursor-pointer"
            onClick={handleImageRemoveClick}
          >
            <span>Remove Picture</span>
          </Dropdown.Item> */}
        </Dropdown.Menu>
      </Dropdown>

      {/* <ProfileMenu
        maxWidth={230}
        minWidth={200}
        popoverOpen={open}
        anchorRef={anchorRef}
        popoverClose={handlePopoverClose}
        noHeader
      >
        <Box pt={1}>
          <label htmlFor="icon-button-file">
            <input
              onChange={handleImageUpdateClick}
              name="imageFile"
              type="file"
              accept="image/*"
              id="icon-button-file"
              style={{ display: 'none' }}
            />
            <StyledSmall>Upload Picture</StyledSmall>
          </label>
          <StyledSmall onClick={handleImageRemoveClick}>
            Remove Picture
          </StyledSmall>
        </Box>
      </ProfileMenu> */}
    </div>
  );
};

export default ImagePopover;
