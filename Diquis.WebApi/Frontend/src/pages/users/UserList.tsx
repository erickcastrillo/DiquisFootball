import { useEffect, useMemo } from "react";
import { observer } from "mobx-react-lite";
import { getCoreRowModel } from "@tanstack/react-table";
import { Button, Card } from "react-bootstrap";

import { useStore } from "stores/store";
import ClientTable from "components/tables/ClientTable/ClientTable";
import RegisterUserModal from "./RegisterUserModal";
import UserColumnShape from "./UserColumnShape";
import PageLayout from "components/PageLayout";
import { useModal } from "hooks";

const UserList = () => {
  const { usersStore } = useStore();
  const { loadAppUsers, appUsersSorted, loadingInitial } = usersStore;
  const { show, onShow, onHide } = useModal();

  const data = useMemo(() => appUsersSorted, [appUsersSorted]);
  const columns = useMemo(() => UserColumnShape, [UserColumnShape]);

  useEffect(() => {
    loadAppUsers();
  }, [loadAppUsers]);

  return (
    <PageLayout
      title="Users"
      action={
        <Button variant="primary" onClick={onShow}>
          New User
        </Button>
      }
    >
      {show && <RegisterUserModal show={show} onHide={onHide} />}
      <Card className="pt-2">
        <Card.Body>
          <ClientTable data={data} columns={columns} getCoreRowModel={getCoreRowModel()} isLoading={loadingInitial} filterFieldPlaceholder="Search users..." />
        </Card.Body>
      </Card>
      <div className="mb-2">
        <small>Client-side pagination with sorting & filtering</small>
      </div>
    </PageLayout>
  );
};

export default observer(UserList);
