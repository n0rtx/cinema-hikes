import { useEffect, useRef } from "react";
import { axiosClient } from "../../shared/api/axiosClient";
import {message } from "antd";
import { useTranslation } from "react-i18next";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Spin } from "antd";
export const GitHubCallbackPage = () => {
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const { t } = useTranslation();
  const started = useRef(false); 

  useEffect(() => {
    if (started.current) return;
    started.current = true;

    const code = params.get("code");
    const state = params.get("state");
    const saved = sessionStorage.getItem("gh_oauth_state");
    sessionStorage.removeItem("gh_oauth_state");

    if (!code || !state || state !== saved) {
      message.error(t("auth.registerFailed"));
      navigate("/login", { replace: true });
      return;
    }

    axiosClient
      .post("/api/auth/github", { code })
      .then(({ data }) => {
        
        localStorage.setItem("authToken", data.token);
        localStorage.setItem("username", data.username);
        localStorage.setItem("email", data.email);
        message.success(t("auth.welcomeAboard"));
        navigate("/", { replace: true });
      })
      .catch((err) => {
        message.error(err.response?.data?.message || t("auth.registerFailed"));
        navigate("/login", { replace: true });
      });
  }, []);

   return <div style={{ minHeight: "100vh", display: "flex", alignItems: "center", justifyContent: "center", backgroundColor: "#141414" }}>
      <Spin size="large" />
    </div>
}
