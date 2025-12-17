import styles from "./Button.module.css";

type Variant = "default" | "primary" | "danger";

export function Button(props: React.ButtonHTMLAttributes<HTMLButtonElement> & { variant?: Variant }) {
  const { variant = "default", className, ...rest } = props;

  const variantClass =
    variant === "primary" ? styles.primary : variant === "danger" ? styles.danger : "";

  return <button {...rest} className={[styles.button, variantClass, className].filter(Boolean).join(" ")} />;
}
