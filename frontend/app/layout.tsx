// frontend/app/layout.tsx

import type { Metadata } from "next";
import "./global.css";

export const metadata: Metadata = {
  title: "The Hyrule Archives",
  description: "A community archive for The Legend of Zelda series.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}