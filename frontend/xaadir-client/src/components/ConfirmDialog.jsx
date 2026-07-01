import Button from "./Button";
import Modal from "./Modal";

export default function ConfirmDialog({ open, message, onCancel, onConfirm }) {
  return (
    <Modal open={open} title="Confirm Delete" onClose={onCancel}>
      <p className="muted">{message}</p>
      <div className="mt-6 flex justify-end gap-3">
        <Button variant="ghost" type="button" onClick={onCancel}>Cancel</Button>
        <Button variant="danger" type="button" onClick={onConfirm}>Delete</Button>
      </div>
    </Modal>
  );
}
