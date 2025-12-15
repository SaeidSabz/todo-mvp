import { TasksPage } from "../features/tasks/TasksPage";

function App() {
  return (
    <main style={{ padding: "2rem", fontFamily: "system-ui, sans-serif" }}>
      <h1>To-Do Task Management MVP</h1>
      <p>Frontend is running.</p>

      <TasksPage />
    </main>
  );
}

export default App;
