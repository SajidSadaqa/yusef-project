"use client"

import { useState, useEffect, useCallback } from "react"
import Link from "next/link"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog"
import { Plus, Search, Edit, Trash2, Loader2, RefreshCw } from "lucide-react"
import { listPorts, deletePort } from "@/lib/services/port.service"
import type { Port } from "@/lib/types/port"
import { useToast } from "@/hooks/use-toast"
import { ApiError } from "@/lib/api/http"
import { getSession } from "@/lib/services/auth.service"

export default function PortsPage() {
  const { toast } = useToast()
  const [ports, setPorts] = useState<Port[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const [showInactive, setShowInactive] = useState(false)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [portToDelete, setPortToDelete] = useState<Port | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [isAdmin, setIsAdmin] = useState(false)

  const fetchPorts = useCallback(async () => {
    try {
      setLoading(true)
      const data = await listPorts({
        activeOnly: !showInactive,
        searchTerm: searchTerm || undefined,
      })
      setPorts(data)
    } catch (err) {
      console.error("Failed to load ports:", err)
      toast({
        variant: "destructive",
        description: "Failed to load ports. Please try again.",
      })
    } finally {
      setLoading(false)
    }
  }, [showInactive, searchTerm, toast])

  useEffect(() => {
    // Determine if current user has Admin role
    const session = getSession()
    const roles = Array.isArray(session?.claims?.roles) ? (session!.claims!.roles as string[]) : []
    setIsAdmin(roles.includes("Admin"))

    fetchPorts()
  }, [fetchPorts])

  const handleSearch = () => {
    fetchPorts()
  }

  const handleDeleteClick = (port: Port) => {
    setPortToDelete(port)
    setDeleteDialogOpen(true)
  }

  const handleDeleteConfirm = async () => {
    if (!portToDelete) return

    try {
      setDeleting(true)
      await deletePort(portToDelete.id)
      toast({ description: "Port deleted successfully." })
      fetchPorts()
      setDeleteDialogOpen(false)
      setPortToDelete(null)
    } catch (err) {
      console.error("Failed to delete port:", err)
      if (err instanceof ApiError) {
        if (err.status === 403) {
          toast({
            variant: "destructive",
            description: "You do not have permission to delete ports (Admin role required).",
          })
        } else {
          toast({
            variant: "destructive",
            description: err.message || "Failed to delete port.",
          })
        }
      } else {
        toast({
          variant: "destructive",
          description: "Failed to delete port. Please try again.",
        })
      }
    } finally {
      setDeleting(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Port Management</h1>
          <p className="text-muted-foreground mt-1">Manage shipping ports and terminals</p>
        </div>
        {isAdmin && (
          <Link href="/admin/ports/create">
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Add Port
            </Button>
          </Link>
        )}
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Ports</CardTitle>
          <CardDescription>
            Search and filter ports. Port codes are generated automatically using UN/LOCODE standard.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex gap-4 mb-6">
            <div className="flex-1 flex gap-2">
              <Input
                placeholder="Search by code, name, or city..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyDown={(e) => e.key === "Enter" && handleSearch()}
              />
              <Button onClick={handleSearch} variant="secondary">
                <Search className="h-4 w-4 mr-2" />
                Search
              </Button>
            </div>
            <Button onClick={fetchPorts} variant="outline" size="icon">
              <RefreshCw className="h-4 w-4" />
            </Button>
            <Button
              onClick={() => setShowInactive(!showInactive)}
              variant={showInactive ? "default" : "outline"}
            >
              {showInactive ? "Show Active Only" : "Show All"}
            </Button>
          </div>

          {loading ? (
            <div className="flex justify-center items-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          ) : ports.length === 0 ? (
            <div className="text-center py-12 text-muted-foreground">
              <p>No ports found.</p>
              <p className="text-sm mt-2">Try adjusting your search or filters.</p>
            </div>
          ) : (
            <div className="border rounded-lg">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Code</TableHead>
                    <TableHead>Name</TableHead>
                    <TableHead>Country</TableHead>
                    <TableHead>City</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {ports.map((port) => (
                    <TableRow key={port.id}>
                      <TableCell className="font-mono font-semibold">{port.code}</TableCell>
                      <TableCell>{port.name}</TableCell>
                      <TableCell>{port.country}</TableCell>
                      <TableCell>{port.city || "—"}</TableCell>
                      <TableCell>
                        <Badge variant={port.isActive ? "default" : "secondary"}>
                          {port.isActive ? "Active" : "Inactive"}
                        </Badge>
                      </TableCell>
                      <TableCell className="text-right">
                        {isAdmin ? (
                          <div className="flex justify-end gap-2">
                            <Link href={`/admin/ports/${port.id}/edit`}>
                              <Button variant="ghost" size="sm">
                                <Edit className="h-4 w-4" />
                              </Button>
                            </Link>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => handleDeleteClick(port)}
                            >
                              <Trash2 className="h-4 w-4 text-destructive" />
                            </Button>
                          </div>
                        ) : (
                          <div className="text-sm text-muted-foreground">—</div>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>

      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Port</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete <strong>{portToDelete?.name} ({portToDelete?.code})</strong>?
              <br /><br />
              This action cannot be undone. If the port is referenced by existing shipments, deletion will fail.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deleting}>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteConfirm}
              disabled={deleting}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleting ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin mr-2" />
                  Deleting...
                </>
              ) : (
                "Delete"
              )}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
