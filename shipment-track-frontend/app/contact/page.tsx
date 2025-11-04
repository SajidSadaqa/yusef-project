import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { MapPin, Phone, Mail, Clock, Truck } from "lucide-react"

export default function ContactPage() {
  return (
    <div className="min-h-screen bg-gradient-to-b from-background to-muted/20">
      {/* Hero Section */}
      <section className="relative py-10 sm:py-12 md:py-16 lg:py-20 bg-gradient-to-br from-primary/5 via-background to-accent/5">
        <div className="container mx-auto px-4 sm:px-6">
          <div className="text-center max-w-3xl mx-auto">
            <h1 className="text-2xl sm:text-3xl md:text-4xl lg:text-5xl font-bold mb-3 sm:mb-4 bg-gradient-to-r from-primary via-primary/80 to-accent bg-clip-text text-transparent text-balance">
              Get In Touch
            </h1>
            <p className="text-sm sm:text-base md:text-lg text-muted-foreground text-pretty">
              Proudly serving Southern California with reliable logistics solutions since 1993
            </p>
          </div>
        </div>
      </section>

      {/* Contact Information & Form */}
      <section className="py-10 sm:py-12 md:py-16 lg:py-20">
        <div className="container mx-auto px-4 sm:px-6">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 lg:gap-10">
            {/* Contact Information */}
            <div className="space-y-5 sm:space-y-6">
              <div>
                <h2 className="text-xl sm:text-2xl md:text-3xl font-bold mb-3 sm:mb-4 text-balance">
                  Contact Information
                </h2>
                <p className="text-sm text-muted-foreground text-pretty">
                  Have questions? We're here to help. Reach out to us through any of the following channels.
                </p>
              </div>

              <div className="space-y-3 sm:space-y-4">
                {/* Address */}
                <Card className="border-l-4 border-l-primary hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
                  <CardContent className="p-4 sm:p-6">
                    <div className="flex items-start gap-3 sm:gap-4">
                      <div className="p-2 sm:p-3 bg-primary/10 rounded-lg">
                        <MapPin className="h-5 w-5 sm:h-6 sm:w-6 text-primary" />
                      </div>
                      <div>
                        <h3 className="font-semibold text-base sm:text-lg mb-1 sm:mb-2">Address</h3>
                        <p className="text-sm sm:text-base text-muted-foreground">
                          4938 Azusa Canyon Rd
                          <br />
                          Irwindale, CA
                        </p>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Phone */}
                <Card className="border-l-4 border-l-accent hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
                  <CardContent className="p-4 sm:p-6">
                    <div className="flex items-start gap-3 sm:gap-4">
                      <div className="p-2 sm:p-3 bg-accent/10 rounded-lg">
                        <Phone className="h-5 w-5 sm:h-6 sm:w-6 text-accent" />
                      </div>
                      <div>
                        <h3 className="font-semibold text-base sm:text-lg mb-1 sm:mb-2">Phone</h3>
                        <a
                          href="tel:+16266881076"
                          className="text-sm sm:text-base text-muted-foreground hover:text-primary transition-colors"
                        >
                          (626) 688-1076
                        </a>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Email */}
                <Card className="border-l-4 border-l-primary hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
                  <CardContent className="p-4 sm:p-6">
                    <div className="flex items-start gap-3 sm:gap-4">
                      <div className="p-2 sm:p-3 bg-primary/10 rounded-lg">
                        <Mail className="h-5 w-5 sm:h-6 sm:w-6 text-primary" />
                      </div>
                      <div>
                        <h3 className="font-semibold text-base sm:text-lg mb-1 sm:mb-2">Email</h3>
                        <a
                          href="mailto:rayvertex1@yahoo.com"
                          className="text-sm sm:text-base text-muted-foreground hover:text-primary transition-colors break-all"
                        >
                          rayvertex1@yahoo.com
                        </a>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Hours */}
                <Card className="border-l-4 border-l-accent hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
                  <CardContent className="p-4 sm:p-6">
                    <div className="flex items-start gap-3 sm:gap-4">
                      <div className="p-2 sm:p-3 bg-accent/10 rounded-lg">
                        <Clock className="h-5 w-5 sm:h-6 sm:w-6 text-accent" />
                      </div>
                      <div>
                        <h3 className="font-semibold text-base sm:text-lg mb-1 sm:mb-2">Business Hours</h3>
                        <p className="text-sm sm:text-base text-muted-foreground">
                          Monday – Saturday
                          <br />
                          5:00 AM – 6:00 PM
                        </p>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Services */}
                <Card className="border-l-4 border-l-primary hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
                  <CardContent className="p-4 sm:p-6">
                    <div className="flex items-start gap-3 sm:gap-4">
                      <div className="p-2 sm:p-3 bg-primary/10 rounded-lg">
                        <Truck className="h-5 w-5 sm:h-6 sm:w-6 text-primary" />
                      </div>
                      <div>
                        <h3 className="font-semibold text-base sm:text-lg mb-1 sm:mb-2">Our Services</h3>
                        <ul className="text-sm sm:text-base text-muted-foreground space-y-1">
                          <li>• Flatbed Hauling</li>
                          <li>• Transloading</li>
                          <li>• Container Storage</li>
                          <li>• Cross-Docking</li>
                        </ul>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              </div>
            </div>

            {/* Contact Form */}
            <div>
              <Card className="border-2 hover:border-primary/50 transition-all duration-300 shadow-lg">
                <CardContent className="p-6 sm:p-8">
                  <h2 className="text-2xl sm:text-3xl font-bold mb-4 sm:mb-6 text-balance">Send Us a Message</h2>
                  <form className="space-y-4 sm:space-y-6">
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div className="space-y-2">
                        <Label htmlFor="firstName" className="text-sm sm:text-base">
                          First Name
                        </Label>
                        <Input id="firstName" placeholder="John" className="h-10 sm:h-11 text-sm sm:text-base" />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="lastName" className="text-sm sm:text-base">
                          Last Name
                        </Label>
                        <Input id="lastName" placeholder="Doe" className="h-10 sm:h-11 text-sm sm:text-base" />
                      </div>
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="email" className="text-sm sm:text-base">
                        Email
                      </Label>
                      <Input
                        id="email"
                        type="email"
                        placeholder="john.doe@example.com"
                        className="h-10 sm:h-11 text-sm sm:text-base"
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="phone" className="text-sm sm:text-base">
                        Phone Number
                      </Label>
                      <Input
                        id="phone"
                        type="tel"
                        placeholder="(555) 123-4567"
                        className="h-10 sm:h-11 text-sm sm:text-base"
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="subject" className="text-sm sm:text-base">
                        Subject
                      </Label>
                      <Input
                        id="subject"
                        placeholder="How can we help you?"
                        className="h-10 sm:h-11 text-sm sm:text-base"
                      />
                    </div>

                    <div className="space-y-2">
                      <Label htmlFor="message" className="text-sm sm:text-base">
                        Message
                      </Label>
                      <Textarea
                        id="message"
                        placeholder="Tell us more about your shipping needs..."
                        rows={5}
                        className="resize-none text-sm sm:text-base"
                      />
                    </div>

                    <Button
                      type="submit"
                      size="lg"
                      className="w-full bg-gradient-to-r from-primary to-accent hover:from-primary/90 hover:to-accent/90 font-semibold shadow-lg hover:shadow-xl transition-all text-sm sm:text-base h-11 sm:h-12"
                    >
                      Send Message
                    </Button>
                  </form>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </section>

      {/* Map Section */}
      <section className="py-10 sm:py-12 md:py-16 bg-muted/30">
        <div className="container mx-auto px-4 sm:px-6">
          <div className="text-center mb-6 sm:mb-8">
            <h2 className="text-xl sm:text-2xl md:text-3xl font-bold mb-2 sm:mb-3 text-balance">Visit Our Location</h2>
            <p className="text-sm text-muted-foreground text-pretty">Find us at our Irwindale facility</p>
          </div>
          <div className="max-w-5xl mx-auto">
            <Card className="overflow-hidden shadow-xl">
              <CardContent className="p-0">
                <iframe
                  src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3302.8!2d-117.9!3d34.1!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzTCsDA2JzAwLjAiTiAxMTfCsDU0JzAwLjAiVw!5e0!3m2!1sen!2sus!4v1234567890"
                  width="100%"
                  height="400"
                  style={{ border: 0 }}
                  allowFullScreen
                  loading="lazy"
                  referrerPolicy="no-referrer-when-downgrade"
                  className="w-full h-[280px] sm:h-[350px] md:h-[400px]"
                />
              </CardContent>
            </Card>
          </div>
        </div>
      </section>
    </div>
  )
}
