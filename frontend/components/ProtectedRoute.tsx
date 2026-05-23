// frontend/components/ProtectedRoute.tsx

"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { CurrentUser, getCurrentUser } from "@/lib/api";

type ProtectedRouteProps = {
  children: React.ReactNode;
};

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const router = useRouter();

  // user starts as undefined because we do not know yet.
  // null means "not logged in."
  // CurrentUser means "logged in."
  const [user, setUser] = useState<CurrentUser | null | undefined>(undefined);

  useEffect(() => {
    async function checkAuth() {
      try {
        const currentUser = await getCurrentUser();

        if (currentUser === null) {
          router.push("/login");
          return;
        }

        setUser(currentUser);
      } catch {
        router.push("/login");
      }
    }

    checkAuth();
  }, [router]);

  // While the auth check is happening, show a loading message.
  if (user === undefined) {
    return <p>Checking sign-in status...</p>;
  }

  // If the user is not logged in, we already redirected to /login.
  // Returning null prevents protected content from flashing on screen.
  if (user === null) {
    return null;
  }

  // If the user exists, show the protected page content.
  return <>{children}</>;
}