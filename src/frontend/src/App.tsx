import { useState } from "react";

function App() {
  const [apiStatus] = useState<"pending" | "ok" | "error">("pending");

  return (
    <main style={{ padding: "2rem", fontFamily: "system-ui, sans-serif" }}>
      <h1>To-Do Task Management MVP</h1>
      <p>Frontend is running.</p>

      <section style={{ marginTop: "1.5rem" }}>
        <h2>Backend API Status</h2>
        {apiStatus === "pending" && <p>(Not connected yet — coming soon.)</p>}
        {apiStatus === "ok" && <p style={{ color: "green" }}>API status: OK</p>}
        {apiStatus === "error" && (
          <p style={{ color: "red" }}>API connection failed.</p>
        )}
      </section>
    </main>
  );
}

export default App;
