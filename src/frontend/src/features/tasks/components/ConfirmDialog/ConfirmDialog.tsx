import styles from "./ConfirmDialog.module.css";
import { Button } from "../../../../ui/Button";

export function ConfirmDialog(props: {
  open: boolean;
  title: string;
  message: string;
  confirmText: string;
  cancelText: string;
  danger?: boolean;
  disableConfirm?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}) {
  if (!props.open) return null;

  return (
    <div className={styles.backdrop} role="dialog" aria-modal="true" aria-label={props.title}>
      <div className={styles.dialog}>
        <div className={styles.header}>
          <h3 className={styles.title}>{props.title}</h3>
        </div>

        <p className={styles.message}>{props.message}</p>

        <div className={styles.actions}>
          <Button type="button" onClick={props.onCancel}>
            {props.cancelText}
          </Button>

          <Button
            type="button"
            variant={props.danger ? "danger" : "primary"}
            onClick={props.onConfirm}
            disabled={props.disableConfirm}
          >
            {props.confirmText}
          </Button>
        </div>
      </div>
    </div>
  );
}
