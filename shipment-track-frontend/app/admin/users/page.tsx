"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Plus, Edit, Trash2, Loader2, ChevronLeft, ChevronRight } from "lucide-react"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import { useToast } from "@/hooks/use-toast"
import { listUsers, createUser, updateUser, deleteUser } from "@/lib/services/user.service"
import { User, UserRole, UserStatus, getUserRoleLabel, getUserStatusLabel, CreateUserPayload, UpdateUserPayload } from "@/lib/types/user"
import { ApiError } from "@/lib/api/http"

type NewUserForm = {
  email: string
  password: string
  firstName: string
  lastName: string
  role: UserRole
}

type EditUserForm = {
  id: string
  email: string
  firstName: string
  lastName: string
  role: UserRole
  status: UserStatus
}

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([])
  const [loading, setLoading] = useState(true)
  const [totalCount, setTotalCount] = useState(0)
  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize] = useState(10)

  const [addDialogOpen, setAddDialogOpen] = useState(false)
  const [editDialogOpen, setEditDialogOpen] = useState(false)
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)

  const [selectedUser, setSelectedUser] = useState<User | null>(null)
  const [newUserForm, setNewUserForm] = useState<NewUserForm>({
    email: "",
    password: "",
    firstName: "",
    lastName: "",
    role: UserRole.Customer,
  })
  const [editForm, setEditForm] = useState<EditUserForm | null>(null)

  const [submitting, setSubmitting] = useState(false)
  const [deleting, setDeleting] = useState(false)

  const { toast } = useToast()

  // Load users on mount and when page changes
  useEffect(() => {
    loadUsers()
  }, [currentPage])

  const loadUsers = async () => {
    setLoading(true)
    try {
      const result = await listUsers({
        page: currentPage,
        pageSize,
      })
      setUsers(result.items)
      setTotalCount(result.totalCount)
    } catch (error) {
      if (error instanceof ApiError) {
        toast({
          title: "Error Loading Users",
          description: error.message,
          variant: "destructive",
        })
      } else {
        toast({
          title: "Error Loading Users",
          description: "An unexpected error occurred",
          variant: "destructive",
        })
      }
    } finally {
      setLoading(false)
    }
  }

  const handleAddUser = () => {
    setNewUserForm({
      email: "",
      password: "",
      firstName: "",
      lastName: "",
      role: UserRole.Customer,
    })
    setAddDialogOpen(true)
  }

  const saveNewUser = async () => {
    if (!newUserForm.email || !newUserForm.password || !newUserForm.firstName || !newUserForm.lastName) {
      toast({
        title: "Validation Error",
        description: "Please fill in all fields",
        variant: "destructive",
      })
      return
    }

    if (newUserForm.password.length < 8) {
      toast({
        title: "Validation Error",
        description: "Password must be at least 8 characters",
        variant: "destructive",
      })
      return
    }

    setSubmitting(true)
    try {
      const payload: CreateUserPayload = {
        email: newUserForm.email,
        password: newUserForm.password,
        firstName: newUserForm.firstName,
        lastName: newUserForm.lastName,
        role: newUserForm.role,
      }

      await createUser(payload)

      toast({
        title: "User Created",
        description: `${newUserForm.firstName} ${newUserForm.lastName} has been created successfully.`,
      })

      setAddDialogOpen(false)
      // Refresh the list
      await loadUsers()
    } catch (error) {
      if (error instanceof ApiError) {
        toast({
          title: "Error Creating User",
          description: error.message,
          variant: "destructive",
        })
      } else {
        toast({
          title: "Error Creating User",
          description: "An unexpected error occurred",
          variant: "destructive",
        })
      }
    } finally {
      setSubmitting(false)
    }
  }

  const handleEdit = (user: User) => {
    setSelectedUser(user)
    setEditForm({
      id: user.id,
      email: user.email,
      firstName: user.firstName,
      lastName: user.lastName,
      role: user.role,
      status: user.status,
    })
    setEditDialogOpen(true)
  }

  const saveEdit = async () => {
    if (!editForm) return

    if (!editForm.email || !editForm.firstName || !editForm.lastName) {
      toast({
        title: "Validation Error",
        description: "Please fill in all fields",
        variant: "destructive",
      })
      return
    }

    setSubmitting(true)
    try {
      const payload: UpdateUserPayload = {
        email: editForm.email,
        firstName: editForm.firstName,
        lastName: editForm.lastName,
        role: editForm.role,
        status: editForm.status,
      }

      await updateUser(editForm.id, payload)

      toast({
        title: "User Updated",
        description: `${editForm.firstName} ${editForm.lastName} has been updated successfully.`,
      })

      setEditDialogOpen(false)
      setEditForm(null)
      setSelectedUser(null)
      // Refresh the list
      await loadUsers()
    } catch (error) {
      if (error instanceof ApiError) {
        toast({
          title: "Error Updating User",
          description: error.message,
          variant: "destructive",
        })
      } else {
        toast({
          title: "Error Updating User",
          description: "An unexpected error occurred",
          variant: "destructive",
        })
      }
    } finally {
      setSubmitting(false)
    }
  }

  const handleDelete = (user: User) => {
    setSelectedUser(user)
    setDeleteDialogOpen(true)
  }

  const confirmDelete = async () => {
    if (!selectedUser) return

    setDeleting(true)
    try {
      await deleteUser(selectedUser.id)

      toast({
        title: "User Deleted",
        description: `${selectedUser.firstName} ${selectedUser.lastName} has been deleted successfully.`,
      })

      setDeleteDialogOpen(false)
      setSelectedUser(null)
      // Refresh the list
      await loadUsers()
    } catch (error) {
      if (error instanceof ApiError) {
        toast({
          title: "Error Deleting User",
          description: error.message,
          variant: "destructive",
        })
      } else {
        toast({
          title: "Error Deleting User",
          description: "An unexpected error occurred",
          variant: "destructive",
        })
      }
    } finally {
      setDeleting(false)
    }
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("en-US", {
      month: "short",
      day: "numeric",
      year: "numeric",
    })
  }

  const totalPages = Math.ceil(totalCount / pageSize)

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-foreground">Users</h1>
          <p className="text-muted-foreground mt-1">Manage user accounts and permissions</p>
        </div>
        <Button className="bg-primary hover:bg-primary/90" onClick={handleAddUser}>
          <Plus className="h-4 w-4 mr-2" />
          Add User
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>All Users ({totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
          ) : users.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No users found
            </div>
          ) : (
            <>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Name</TableHead>
                    <TableHead>Email</TableHead>
                    <TableHead>Role</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Joined</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {users.map((user) => (
                    <TableRow key={user.id}>
                      <TableCell className="font-medium">
                        {user.firstName} {user.lastName}
                      </TableCell>
                      <TableCell>{user.email}</TableCell>
                      <TableCell>
                        <Badge variant={user.role === UserRole.Admin ? "default" : "secondary"}>
                          {getUserRoleLabel(user.role)}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <Badge
                          className={
                            user.status === UserStatus.Active
                              ? "bg-green-500 text-white"
                              : user.status === UserStatus.Suspended
                              ? "bg-red-500 text-white"
                              : "bg-gray-500 text-white"
                          }
                        >
                          {getUserStatusLabel(user.status)}
                        </Badge>
                      </TableCell>
                      <TableCell className="text-sm text-muted-foreground">
                        {formatDate(user.createdAt)}
                      </TableCell>
                      <TableCell className="text-right">
                        <div className="flex items-center justify-end gap-2">
                          <Button variant="ghost" size="sm" onClick={() => handleEdit(user)}>
                            <Edit className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="sm"
                            className="text-destructive hover:text-destructive"
                            onClick={() => handleDelete(user)}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>

              {totalPages > 1 && (
                <div className="flex items-center justify-between mt-4">
                  <p className="text-sm text-muted-foreground">
                    Page {currentPage} of {totalPages}
                  </p>
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                      disabled={currentPage === 1 || loading}
                    >
                      <ChevronLeft className="h-4 w-4 mr-1" />
                      Previous
                    </Button>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                      disabled={currentPage === totalPages || loading}
                    >
                      Next
                      <ChevronRight className="h-4 w-4 ml-1" />
                    </Button>
                  </div>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>

      <Dialog open={addDialogOpen} onOpenChange={setAddDialogOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>Add New User</DialogTitle>
            <DialogDescription>Create a new user account</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="new-firstName">First Name</Label>
              <Input
                id="new-firstName"
                placeholder="Enter first name"
                value={newUserForm.firstName}
                onChange={(e) => setNewUserForm({ ...newUserForm, firstName: e.target.value })}
                disabled={submitting}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="new-lastName">Last Name</Label>
              <Input
                id="new-lastName"
                placeholder="Enter last name"
                value={newUserForm.lastName}
                onChange={(e) => setNewUserForm({ ...newUserForm, lastName: e.target.value })}
                disabled={submitting}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="new-email">Email</Label>
              <Input
                id="new-email"
                type="email"
                placeholder="Enter email address"
                value={newUserForm.email}
                onChange={(e) => setNewUserForm({ ...newUserForm, email: e.target.value })}
                disabled={submitting}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="new-password">Password</Label>
              <Input
                id="new-password"
                type="password"
                placeholder="Enter password (min 8 characters)"
                value={newUserForm.password}
                onChange={(e) => setNewUserForm({ ...newUserForm, password: e.target.value })}
                disabled={submitting}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="new-role">Role</Label>
              <Select
                value={newUserForm.role.toString()}
                onValueChange={(value) => setNewUserForm({ ...newUserForm, role: parseInt(value) as UserRole })}
                disabled={submitting}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={UserRole.Customer.toString()}>Customer</SelectItem>
                  <SelectItem value={UserRole.Staff.toString()}>Staff</SelectItem>
                  <SelectItem value={UserRole.Admin.toString()}>Admin</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setAddDialogOpen(false)} disabled={submitting}>
              Cancel
            </Button>
            <Button onClick={saveNewUser} disabled={submitting}>
              {submitting && <Loader2 className="h-4 w-4 mr-2 animate-spin" />}
              Add User
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>Edit User</DialogTitle>
            <DialogDescription>Update user information</DialogDescription>
          </DialogHeader>
          {editForm && (
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="edit-firstName">First Name</Label>
                <Input
                  id="edit-firstName"
                  value={editForm.firstName}
                  onChange={(e) => setEditForm({ ...editForm, firstName: e.target.value })}
                  disabled={submitting}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="edit-lastName">Last Name</Label>
                <Input
                  id="edit-lastName"
                  value={editForm.lastName}
                  onChange={(e) => setEditForm({ ...editForm, lastName: e.target.value })}
                  disabled={submitting}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="edit-email">Email</Label>
                <Input
                  id="edit-email"
                  type="email"
                  value={editForm.email}
                  onChange={(e) => setEditForm({ ...editForm, email: e.target.value })}
                  disabled={submitting}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="edit-role">Role</Label>
                <Select
                  value={editForm.role.toString()}
                  onValueChange={(value) => setEditForm({ ...editForm, role: parseInt(value) as UserRole })}
                  disabled={submitting}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value={UserRole.Customer.toString()}>Customer</SelectItem>
                    <SelectItem value={UserRole.Staff.toString()}>Staff</SelectItem>
                    <SelectItem value={UserRole.Admin.toString()}>Admin</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="edit-status">Status</Label>
                <Select
                  value={editForm.status.toString()}
                  onValueChange={(value) => setEditForm({ ...editForm, status: parseInt(value) as UserStatus })}
                  disabled={submitting}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value={UserStatus.Active.toString()}>Active</SelectItem>
                    <SelectItem value={UserStatus.Inactive.toString()}>Inactive</SelectItem>
                    <SelectItem value={UserStatus.Suspended.toString()}>Suspended</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setEditDialogOpen(false)} disabled={submitting}>
              Cancel
            </Button>
            <Button onClick={saveEdit} disabled={submitting}>
              {submitting && <Loader2 className="h-4 w-4 mr-2 animate-spin" />}
              Save Changes
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This will permanently delete {selectedUser?.firstName} {selectedUser?.lastName}'s account. This action
              cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deleting}>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              disabled={deleting}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleting && <Loader2 className="h-4 w-4 mr-2 animate-spin" />}
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
