import { useEffect, useState } from "react";

type ApiStatus = "pending" | "ok" | "error";

function App() {
  const [apiStatus, setApiStatus] = useState<ApiStatus>("pending");
  const [apiMessage, setApiMessage] = useState<string | null>(null);

  useEffect(() => {
    const apiBaseUrl = import.meta.env.VITE_API_BASE_URL as string | undefined;

    if (!apiBaseUrl) {
      setApiStatus("error");
      setApiMessage("VITE_API_BASE_URL is not configured.");
      return;
    }

    const fetchHealth = async () => {
      try {
        const response = await fetch(`${apiBaseUrl}/api/health`);

        if (!response.ok) {
          setApiStatus("error");
          setApiMessage(`HTTP error ${response.status}`);
          return;
        }

        const data = await response.json();
        setApiStatus("ok");
        setApiMessage(`status: ${data.status}, time: ${data.timestampUtc}`);
      } catch (error) {
        setApiStatus("error");
        setApiMessage("Network error while calling /api/health.");
      }
    };

    fetchHealth();
  }, []);

  return (
    <main style={{ padding: "2rem", fontFamily: "system-ui, sans-serif" }}>
      <h1>To-Do Task Management MVP</h1>
      <p>Frontend is running.</p>

      <section style={{ marginTop: "1.5rem" }}>
        <h2>Backend API Status</h2>
        {apiStatus === "pending" && <p>Checking API health...</p>}
        {apiStatus === "ok" && (
          <p style={{ color: "green" }}>
            API is reachable ({apiMessage ?? "OK"}).
          </p>
        )}
        {apiStatus === "error" && (
          <p style={{ color: "red" }}>
            API check failed: {apiMessage ?? "Unknown error"}
          </p>
        )}
      </section>
    </main>
  );
}

export default App;
