import { Link } from "react-router-dom";
import { Dropdown } from "react-bootstrap";
import clsx from "clsx";

import { useToggle } from "hooks";
import { ProfileOption } from "layout/nav/types";
import { useStore } from "stores/store";
import { Avatar } from "components/ui";
import { getInitials } from "utils";

type ProfileDropdownProps = {
  menuItems: Array<ProfileOption>;
  userImage: string;
  username: string;
};

const ProfileDropdown = ({ username, menuItems, userImage }: ProfileDropdownProps) => {
  const [isOpen, toggleDropdown] = useToggle();
  const { accountStore } = useStore();
  const { logout, currentUser } = accountStore;

  return (
    <Dropdown show={isOpen} as="li" onToggle={toggleDropdown}>
      <Dropdown.Toggle variant="link" id="dropdown-profile" as={Link} to="#" onClick={toggleDropdown} className="nav-link dropdown-toggle arrow-none nav-user px-2">
        <span className="account-user-avatar">
          <Avatar src={userImage} text={getInitials(currentUser?.firstName || "", currentUser?.lastName || "")} height="3rem" width="3rem" />
        </span>
        <span className="d-lg-flex flex-column gap-1 d-none">
          <h5 className="my-0">{username}</h5>
          <h6 className="my-0 fw-normal text-start text-capitalize">{currentUser?.roleId} User</h6>
        </span>
      </Dropdown.Toggle>
      <Dropdown.Menu align={"end"} className="dropdown-menu-animated topbar-dropdown-menu profile-dropdown">
        <div onClick={toggleDropdown}>
          {menuItems.map((item, i) => {
            return item.label === "Logout" ? (
              <div className="dropdown-item notify-item" key={i + "-profile-menu"} onClick={logout}>
                <i className={clsx(item.icon, "me-1")}></i>
                <span>{item.label}</span>
              </div>
            ) : (
              <Link to={item.redirectTo || "#"} className="dropdown-item notify-item" key={i + "-profile-menu"}>
                <i className={clsx(item.icon, "me-1")}></i>
                <span>{item.label}</span>
              </Link>
            );
          })}
        </div>
      </Dropdown.Menu>
    </Dropdown>
  );
};

export default ProfileDropdown;
