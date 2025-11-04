import { Card, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Truck, Package, Clock, Shield, MapPin, Users } from "lucide-react"

export default function ServicesPage() {
  return (
    <div className="container mx-auto px-4 py-12 sm:py-14 md:py-16">
      <div className="max-w-5xl mx-auto">
        <div className="text-center mb-12 sm:mb-14">
          <h1 className="text-3xl sm:text-4xl md:text-5xl font-bold mb-3 sm:mb-4 text-foreground">Our Services</h1>
          <p className="text-muted-foreground text-base sm:text-lg leading-relaxed max-w-3xl mx-auto">
            Comprehensive transportation and logistics solutions tailored to your business needs
          </p>
        </div>

        <div className="grid gap-5">
          <Card className="border-2 border-primary/20 hover:border-primary/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-primary/5 to-transparent">
            <CardHeader className="p-5 sm:p-6">
              <div className="flex items-start gap-4 sm:gap-5">
                <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-primary to-primary/70 flex items-center justify-center flex-shrink-0 shadow-md">
                  <Truck className="h-5 w-5 text-white" />
                </div>
                <div>
                  <CardTitle className="text-lg sm:text-xl mb-2">Full Truckload (FTL)</CardTitle>
                  <CardDescription className="text-sm leading-relaxed">
                    Dedicated transportation for large shipments requiring exclusive use of a trailer. Perfect for
                    businesses with high-volume freight needs and time-sensitive deliveries.
                  </CardDescription>
                </div>
              </div>
            </CardHeader>
          </Card>

          <Card className="border-2 border-accent/20 hover:border-accent/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-accent/5 to-transparent">
            <CardHeader className="p-5 sm:p-6">
              <div className="flex items-start gap-4 sm:gap-5">
                <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-accent to-accent/70 flex items-center justify-center flex-shrink-0 shadow-md">
                  <Package className="h-5 w-5 text-white" />
                </div>
                <div>
                  <CardTitle className="text-lg sm:text-xl mb-2">Less Than Truckload (LTL)</CardTitle>
                  <CardDescription className="text-sm leading-relaxed">
                    Cost-effective shipping for smaller freight that doesn't require a full trailer. Share space with
                    other shipments while maintaining security and reliability.
                  </CardDescription>
                </div>
              </div>
            </CardHeader>
          </Card>

          <Card className="border-2 border-secondary/20 hover:border-secondary/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-secondary/5 to-transparent">
            <CardHeader className="p-5 sm:p-6">
              <div className="flex items-start gap-4 sm:gap-5">
                <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-secondary to-secondary/70 flex items-center justify-center flex-shrink-0 shadow-md">
                  <Clock className="h-5 w-5 text-white" />
                </div>
                <div>
                  <CardTitle className="text-lg sm:text-xl mb-2">Expedited Shipping</CardTitle>
                  <CardDescription className="text-sm leading-relaxed">
                    Time-critical deliveries with guaranteed arrival times for urgent shipments. Our priority service
                    ensures your cargo arrives exactly when you need it.
                  </CardDescription>
                </div>
              </div>
            </CardHeader>
          </Card>

          <Card className="border-2 border-blue-500/20 hover:border-blue-500/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-blue-500/5 to-transparent">
            <CardHeader className="p-5 sm:p-6">
              <div className="flex items-start gap-4 sm:gap-5">
                <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center flex-shrink-0 shadow-md">
                  <MapPin className="h-5 w-5 text-white" />
                </div>
                <div>
                  <CardTitle className="text-lg sm:text-xl mb-2">Regional Distribution</CardTitle>
                  <CardDescription className="text-sm leading-relaxed">
                    Efficient distribution services across California and neighboring states. Leverage our extensive
                    network for seamless regional logistics.
                  </CardDescription>
                </div>
              </div>
            </CardHeader>
          </Card>

          <Card className="border-2 border-emerald-500/20 hover:border-emerald-500/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-emerald-500/5 to-transparent">
            <CardHeader className="p-5 sm:p-6">
              <div className="flex items-start gap-4 sm:gap-5">
                <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-emerald-500 to-emerald-600 flex items-center justify-center flex-shrink-0 shadow-md">
                  <Shield className="h-5 w-5 text-white" />
                </div>
                <div>
                  <CardTitle className="text-lg sm:text-xl mb-2">Specialized Freight</CardTitle>
                  <CardDescription className="text-sm leading-relaxed">
                    Handling of temperature-controlled, hazardous, and oversized cargo with specialized equipment.
                    Expert care for your most demanding shipments.
                  </CardDescription>
                </div>
              </div>
            </CardHeader>
          </Card>

          <Card className="border-2 border-amber-500/20 hover:border-amber-500/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-amber-500/5 to-transparent">
            <CardHeader className="p-5 sm:p-6">
              <div className="flex items-start gap-4 sm:gap-5">
                <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-amber-500 to-amber-600 flex items-center justify-center flex-shrink-0 shadow-md">
                  <Users className="h-5 w-5 text-white" />
                </div>
                <div>
                  <CardTitle className="text-lg sm:text-xl mb-2">Dedicated Fleet Services</CardTitle>
                  <CardDescription className="text-sm leading-relaxed">
                    Customized fleet solutions with dedicated drivers and equipment for your business. Consistent
                    service with vehicles branded to your specifications.
                  </CardDescription>
                </div>
              </div>
            </CardHeader>
          </Card>
        </div>
      </div>
    </div>
  )
}
