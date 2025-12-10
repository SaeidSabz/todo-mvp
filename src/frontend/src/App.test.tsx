import { render, screen } from "@testing-library/react";
import { test, expect } from "vitest";
import App from "./App";

test('renders "Frontend is running" text', () => {
  render(<App />);
  const text = screen.getByText(/Frontend is running/i);
  expect(text).toBeInTheDocument();
});
