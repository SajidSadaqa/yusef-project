import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import { Archive } from "lucide-react"

export default function SettingsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-foreground">Settings</h1>
        <p className="text-muted-foreground mt-1">Manage your application settings and preferences</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Company Information</CardTitle>
          <CardDescription>Update your company details</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="companyName">Company Name</Label>
            <Input id="companyName" defaultValue="Vertex Transport" />
          </div>
          <div className="space-y-2">
            <Label htmlFor="companyEmail">Contact Email</Label>
            <Input id="companyEmail" type="email" defaultValue="info@vertextransport.com" />
          </div>
          <div className="space-y-2">
            <Label htmlFor="companyPhone">Phone Number</Label>
            <Input id="companyPhone" type="tel" defaultValue="(555) 123-4567" />
          </div>
          <div className="space-y-2">
            <Label htmlFor="companyAddress">Address</Label>
            <Input id="companyAddress" defaultValue="Irwindale, CA" />
          </div>
          <Button className="bg-primary hover:bg-primary/90">Save Changes</Button>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>System Settings</CardTitle>
          <CardDescription>Configure system-wide settings</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center justify-between">
            <div className="space-y-0.5">
              <Label>Maintenance Mode</Label>
              <p className="text-sm text-muted-foreground">Temporarily disable public access</p>
            </div>
            <Switch />
          </div>
          <div className="flex items-center justify-between">
            <div className="space-y-0.5">
              <Label>Auto-Archive</Label>
              <p className="text-sm text-muted-foreground">Automatically archive delivered shipments after 30 days</p>
            </div>
            <Switch defaultChecked />
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Archive Management</CardTitle>
          <CardDescription>View and manage archived shipments</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-center justify-between p-4 bg-muted/50 rounded-lg border">
            <div>
              <p className="font-medium text-foreground">Archived Shipments</p>
              <p className="text-sm text-muted-foreground">View all delivered and archived shipments</p>
            </div>
            <Button variant="outline" className="gap-2 bg-transparent">
              <Archive className="h-4 w-4" />
              View Archive
            </Button>
          </div>
          <p className="text-xs text-muted-foreground">
            Archived shipments are stored for 90 days before permanent deletion
          </p>
        </CardContent>
      </Card>
    </div>
  )
}
