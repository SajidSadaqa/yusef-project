import type React from "react"
import type { Metadata } from "next"
import { Inter } from "next/font/google"
import "./globals.css"
import { Header } from "@/components/header"
import { Footer } from "@/components/footer"
import { ThemeProvider } from "@/components/theme-provider"

const inter = Inter({ subsets: ["latin"] })

export const metadata: Metadata = {
  metadataBase: new URL(process.env.NEXT_PUBLIC_SITE_URL || 'https://www.vertextransport.com'),
  title: {
    default: "Vertex Transport - Professional Logistics & Shipping Solutions",
    template: "%s | Vertex Transport"
  },
  description: "Professional international shipping and logistics services. Track your shipments in real-time with Vertex Transport. Reliable freight forwarding, port-to-port shipping, and customs clearance.",
  keywords: ["logistics", "shipping", "freight forwarding", "shipment tracking", "international shipping", "cargo", "port services", "customs clearance"],
  authors: [{ name: "Vertex Transport" }],
  creator: "Vertex Transport",
  publisher: "Vertex Transport",
  formatDetection: {
    email: false,
    address: false,
    telephone: false,
  },
  openGraph: {
    type: "website",
    locale: "en_US",
    url: "/",
    title: "Vertex Transport - Professional Logistics & Shipping Solutions",
    description: "Professional international shipping and logistics services. Track your shipments in real-time with Vertex Transport.",
    siteName: "Vertex Transport",
    images: [
      {
        url: "/og-image.png",
        width: 1200,
        height: 630,
        alt: "Vertex Transport - Professional Logistics & Shipping"
      }
    ]
  },
  twitter: {
    card: "summary_large_image",
    title: "Vertex Transport - Professional Logistics & Shipping Solutions",
    description: "Professional international shipping and logistics services. Track your shipments in real-time.",
    images: ["/og-image.png"],
  },
  robots: {
    index: true,
    follow: true,
    googleBot: {
      index: true,
      follow: true,
      'max-video-preview': -1,
      'max-image-preview': 'large',
      'max-snippet': -1,
    },
  },
  icons: {
    icon: [
      { url: "/favicon.ico" },
      { url: "/favicon-16x16.png", sizes: "16x16", type: "image/png" },
      { url: "/favicon-32x32.png", sizes: "32x32", type: "image/png" },
    ],
    apple: [
      { url: "/apple-touch-icon.png", sizes: "180x180", type: "image/png" },
    ],
  },
  manifest: "/site.webmanifest",
  verification: {
    // Add your verification codes when you have them
    // google: 'your-google-verification-code',
    // bing: 'your-bing-verification-code',
  },
}

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className={inter.className}>
        <ThemeProvider attribute="class" defaultTheme="light" enableSystem={false}>
          <div className="flex min-h-screen flex-col">
            <Header />
            <main className="flex-1">
              {children}
            </main>
            <Footer />
          </div>
        </ThemeProvider>
      </body>
    </html>
  )
}
