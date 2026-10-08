import { AntdThemeProvider } from "./providers/AntdThemeProvider";
import { QueryProvider } from "./providers/QueryProvider";
import { RouterProvider } from "react-router-dom";
import { router } from "./routers/index";
import   "../shared/i18n";
import "./styles/global.css";

export function App() {
  return (
    <QueryProvider>
      <AntdThemeProvider>
        <RouterProvider router={router} />
      </AntdThemeProvider>
    </QueryProvider>
  );
}