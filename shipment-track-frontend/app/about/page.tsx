import { Card, CardContent } from "@/components/ui/card"
import Image from "next/image"
import { Award, Users, Truck, Target } from "lucide-react"

export default function AboutPage() {
  return (
    <div className="container mx-auto px-4 py-12 sm:py-14 md:py-16">
      <div className="max-w-5xl mx-auto">
        <div className="text-center mb-12 sm:mb-14">
          <h1 className="text-3xl sm:text-4xl md:text-5xl font-bold mb-3 sm:mb-4 text-foreground">
            About Vertex Transport
          </h1>
          <p className="text-muted-foreground text-base sm:text-lg leading-relaxed max-w-3xl mx-auto">
            Your trusted partner in professional transportation and logistics since 1990
          </p>
        </div>

        <Card className="mb-8 border-2 border-primary/20 hover:border-primary/40 transition-all hover:shadow-lg bg-gradient-to-br from-primary/5 to-transparent">
          <CardContent className="p-6 sm:p-8">
            <div className="flex justify-center mb-6">
              <Image src="/vertex-logo.png" alt="Vertex Transport" width={240} height={80} className="h-16 w-auto" />
            </div>
            <p className="text-muted-foreground leading-relaxed text-sm sm:text-base mb-4">
              Based in Irwindale, California, Vertex Transport has been providing reliable transportation and logistics
              services throughout the region for over three decades. We specialize in delivering excellence through our
              unwavering commitment to safety, efficiency, and customer satisfaction.
            </p>
            <p className="text-muted-foreground leading-relaxed text-sm sm:text-base">
              Our modern fleet and experienced team ensure that your cargo arrives on time, every time. With
              state-of-the-art tracking technology and a customer-first approach, we've built a reputation as one of
              California's most trusted transportation providers.
            </p>
          </CardContent>
        </Card>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
          <Card className="border-2 border-primary/20 hover:border-primary/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-primary/5 to-transparent">
            <CardContent className="p-5 sm:p-6">
              <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-primary to-primary/70 flex items-center justify-center mb-4 shadow-md">
                <Award className="h-5 w-5 text-white" />
              </div>
              <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Industry Excellence</h3>
              <p className="text-muted-foreground leading-relaxed text-sm">
                Recognized for outstanding service quality and safety standards in the transportation industry with
                multiple awards and certifications.
              </p>
            </CardContent>
          </Card>

          <Card className="border-2 border-accent/20 hover:border-accent/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-accent/5 to-transparent">
            <CardContent className="p-5 sm:p-6">
              <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-accent to-accent/70 flex items-center justify-center mb-4 shadow-md">
                <Users className="h-5 w-5 text-white" />
              </div>
              <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Experienced Team</h3>
              <p className="text-muted-foreground leading-relaxed text-sm">
                Professional drivers and logistics experts with decades of combined experience, dedicated to delivering
                exceptional service every time.
              </p>
            </CardContent>
          </Card>

          <Card className="border-2 border-secondary/20 hover:border-secondary/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-secondary/5 to-transparent">
            <CardContent className="p-5 sm:p-6">
              <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-secondary to-secondary/70 flex items-center justify-center mb-4 shadow-md">
                <Truck className="h-5 w-5 text-white" />
              </div>
              <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Modern Fleet</h3>
              <p className="text-muted-foreground leading-relaxed text-sm">
                Well-maintained vehicles equipped with the latest technology for safe and efficient transport, regularly
                serviced to ensure reliability.
              </p>
            </CardContent>
          </Card>

          <Card className="border-2 border-blue-500/20 hover:border-blue-500/50 transition-all hover:shadow-lg hover:-translate-y-1 bg-gradient-to-br from-blue-500/5 to-transparent">
            <CardContent className="p-5 sm:p-6">
              <div className="h-11 w-11 rounded-lg bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center mb-4 shadow-md">
                <Target className="h-5 w-5 text-white" />
              </div>
              <h3 className="text-lg sm:text-xl font-semibold mb-2 text-foreground">Customer Focus</h3>
              <p className="text-muted-foreground leading-relaxed text-sm">
                Tailored solutions and responsive 24/7 support to meet your unique transportation needs with
                personalized service and attention to detail.
              </p>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
