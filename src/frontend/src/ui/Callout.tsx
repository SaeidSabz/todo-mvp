import styles from "./Callout.module.css";

export function Callout(props: { title: string; children?: React.ReactNode; actions?: React.ReactNode }) {
  return (
    <section className={styles.callout} role="status">
      <div className={styles.row}>
        <h3 className={styles.title}>{props.title}</h3>
        {props.actions}
      </div>
      {props.children ? <p className={styles.body}>{props.children}</p> : null}
    </section>
  );
}
