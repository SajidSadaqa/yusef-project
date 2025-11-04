import Link from "next/link"
import Image from "next/image"
import { Facebook, Twitter, Linkedin, Instagram } from "lucide-react"

export function Footer() {
  return (
    <footer className="border-t border-border/40 bg-linear-to-b from-muted/30 to-muted/60">
      <div className="container mx-auto px-6 py-16">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-12">
          <div className="space-y-6">
            <Image src="/vertex-logo.png" alt="Vertex Transport" width={180} height={60} className="h-12 w-auto" />
            <p className="text-sm text-muted-foreground leading-relaxed max-w-xs">
              Professional transportation and logistics services you can trust. Expert hauling since 1990.
            </p>
            <div className="flex items-center gap-3">
              <Link
                href="#"
                className="p-2 rounded-full bg-primary/10 text-primary hover:bg-primary hover:text-primary-foreground transition-all hover:scale-110"
              >
                <Facebook className="h-4 w-4" />
              </Link>
              <Link
                href="#"
                className="p-2 rounded-full bg-primary/10 text-primary hover:bg-primary hover:text-primary-foreground transition-all hover:scale-110"
              >
                <Twitter className="h-4 w-4" />
              </Link>
              <Link
                href="#"
                className="p-2 rounded-full bg-primary/10 text-primary hover:bg-primary hover:text-primary-foreground transition-all hover:scale-110"
              >
                <Linkedin className="h-4 w-4" />
              </Link>
              <Link
                href="#"
                className="p-2 rounded-full bg-primary/10 text-primary hover:bg-primary hover:text-primary-foreground transition-all hover:scale-110"
              >
                <Instagram className="h-4 w-4" />
              </Link>
            </div>
          </div>

          <div>
            <h3 className="font-bold text-lg mb-6 text-foreground">Quick Links</h3>
            <ul className="space-y-3">
              <li>
                <Link
                  href="/"
                  className="text-sm text-muted-foreground hover:text-primary hover:translate-x-1 transition-all inline-block"
                >
                  Track Shipment
                </Link>
              </li>
              <li>
                <Link
                  href="/services"
                  className="text-sm text-muted-foreground hover:text-primary hover:translate-x-1 transition-all inline-block"
                >
                  Our Services
                </Link>
              </li>
              <li>
                <Link
                  href="/about"
                  className="text-sm text-muted-foreground hover:text-primary hover:translate-x-1 transition-all inline-block"
                >
                  About Us
                </Link>
              </li>
            </ul>
          </div>

          <div>
            <h3 className="font-bold text-lg mb-6 text-foreground">Support</h3>
            <ul className="space-y-3">
              <li>
                <Link
                  href="/login"
                  className="text-sm text-muted-foreground hover:text-primary hover:translate-x-1 transition-all inline-block"
                >
                  Customer Login
                </Link>
              </li>
              <li>
                <Link
                  href="/admin"
                  className="text-sm text-muted-foreground hover:text-primary hover:translate-x-1 transition-all inline-block"
                >
                  Admin Portal
                </Link>
              </li>
            </ul>
          </div>

          <div>
            <h3 className="font-bold text-lg mb-6 text-foreground">Contact</h3>
            <ul className="space-y-4">
              <li className="flex items-start gap-3 text-sm text-muted-foreground group">
                <span className="text-base">📍</span>
                <div className="group-hover:text-foreground transition-colors">
                  <div className="font-medium text-foreground">Vertex Transportation</div>
                  <div>4938 Azusa Canyon Rd, Irwindale, CA</div>
                </div>
              </li>
              <li className="flex items-center gap-3 text-sm text-muted-foreground group">
                <span className="text-base">📞</span>
                <a href="tel:6266881076" className="group-hover:text-foreground transition-colors">
                  (626) 688-1076
                </a>
              </li>
              <li className="flex items-center gap-3 text-sm text-muted-foreground group">
                <span className="text-base">✉️</span>
                <a href="mailto:rayvertex1@yahoo.com" className="group-hover:text-foreground transition-colors">
                  rayvertex1@yahoo.com
                </a>
              </li>
            </ul>
          </div>
        </div>

        <div className="mt-12 pt-8 border-t border-border/40 flex flex-col md:flex-row items-center justify-between gap-4">
          <p className="text-sm text-muted-foreground">
            &copy; {new Date().getFullYear()} Vertex Transport. All rights reserved.
          </p>
          <div className="flex items-center gap-6 text-sm text-muted-foreground">
            <Link href="#" className="hover:text-primary transition-colors">
              Privacy Policy
            </Link>
            <Link href="#" className="hover:text-primary transition-colors">
              Terms of Service
            </Link>
          </div>
        </div>
      </div>
    </footer>
  )
}
