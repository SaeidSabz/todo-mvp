import { TasksPage } from "../features/tasks/TasksPage";
import styles from "./App.module.css";

export default function App() {
  return (
    <div className={styles.shell}>
      <header className={styles.topbar}>
        <div className={styles.container}>
          <div className={styles.brand}>
            <div className={styles.logo} aria-hidden="true">✓</div>
            <div>
              <div className={styles.title}>To-Do MVP</div>
              <div className={styles.subtitle}>Simple task management</div>
            </div>
          </div>
        </div>
      </header>

      <main className={styles.container}>
        <TasksPage />
      </main>

      <footer className={styles.footer}>
        <div className={styles.container}>
          <span>© {new Date().getFullYear()} To-Do MVP</span>
        </div>
      </footer>
    </div>
  );
}
