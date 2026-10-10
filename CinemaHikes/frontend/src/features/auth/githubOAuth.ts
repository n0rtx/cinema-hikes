export const startGitHubLogin = () => {
  const state = crypto.randomUUID();
  sessionStorage.setItem("gh_oauth_state", state); 

  const params = new URLSearchParams({
    client_id: import.meta.env.VITE_GITHUB_CLIENT_ID,
    redirect_uri: `${window.location.origin}/auth/github/callback`,
    scope: "read:user user:email", 
    state,
  });
  window.location.href = `https://github.com/login/oauth/authorize?${params}`;
};