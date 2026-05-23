"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";

export default function AuthCallbackPage() {
  const router = useRouter();

  useEffect(() => {
    // The backend has already handled the Google callback.
    // This page just gives React a place to land after login.
    router.replace("/dashboard");
  }, [router]);

  return (
    <main>
      <h1>Signing you in...</h1>
      <p>Please wait while we send you to your dashboard.</p>
    </main>
  );
}