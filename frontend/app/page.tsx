"use client"

import { useState } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import {
  Eye,
  EyeOff,
  Send,
  Download,
  Plus,
  ArrowUpRight,
  ArrowDownLeft,
  CreditCard,
  Settings,
  Bell,
  User,
} from "lucide-react"

export default function WalletDashboard() {
  const [showBalance, setShowBalance] = useState(true)
  const [balance] = useState(2847.5)

  const transactions = [
    {
      id: 1,
      type: "received",
      amount: 250.0,
      description: "Pago de Juan Pérez",
      date: "Hoy, 14:30",
      status: "completed",
    },
    {
      id: 2,
      type: "sent",
      amount: -85.5,
      description: "Transferencia a María García",
      date: "Ayer, 09:15",
      status: "completed",
    },
    {
      id: 3,
      type: "received",
      amount: 1200.0,
      description: "Depósito de salario",
      date: "2 días atrás",
      status: "completed",
    },
    {
      id: 4,
      type: "sent",
      amount: -45.0,
      description: "Pago de servicios",
      date: "3 días atrás",
      status: "pending",
    },
  ]

  const quickActions = [
    { icon: Send, label: "Enviar", color: "bg-blue-500 hover:bg-blue-600" },
    { icon: Download, label: "Recibir", color: "bg-green-500 hover:bg-green-600" },
    { icon: Plus, label: "Recargar", color: "bg-purple-500 hover:bg-purple-600" },
    { icon: CreditCard, label: "Tarjetas", color: "bg-orange-500 hover:bg-orange-600" },
  ]

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center space-x-4">
              <div className="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
                <span className="text-white font-bold text-sm">W</span>
              </div>
              <h1 className="text-xl font-semibold text-gray-900">Mi Billetera</h1>
            </div>
            <div className="flex items-center space-x-4">
              <Button variant="ghost" size="icon">
                <Bell className="h-5 w-5" />
              </Button>
              <Button variant="ghost" size="icon">
                <Settings className="h-5 w-5" />
              </Button>
              <Avatar className="h-8 w-8">
                <AvatarImage src="/placeholder.svg?height=32&width=32" />
                <AvatarFallback>
                  <User className="h-4 w-4" />
                </AvatarFallback>
              </Avatar>
            </div>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Balance Card */}
          <div className="lg:col-span-2">
            <Card className="bg-gradient-to-r from-blue-600 to-purple-700 text-white border-0 shadow-xl">
              <CardHeader className="pb-2">
                <div className="flex justify-between items-start">
                  <div>
                    <CardTitle className="text-lg font-medium opacity-90">Balance Total</CardTitle>
                    <div className="flex items-center space-x-2 mt-2">
                      <span className="text-3xl font-bold">
                        {showBalance ? `$${balance.toLocaleString("es-ES", { minimumFractionDigits: 2 })}` : "••••••"}
                      </span>
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => setShowBalance(!showBalance)}
                        className="text-white hover:bg-white/20 h-8 w-8"
                      >
                        {showBalance ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                      </Button>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-sm opacity-90">Cuenta Principal</p>
                    <p className="text-xs opacity-75">**** 1234</p>
                  </div>
                </div>
              </CardHeader>
              <CardContent className="pt-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm opacity-90">Ingresos del mes</p>
                    <p className="text-lg font-semibold">$1,450.00</p>
                  </div>
                  <div>
                    <p className="text-sm opacity-90">Gastos del mes</p>
                    <p className="text-lg font-semibold">$897.50</p>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Quick Actions */}
            <div className="mt-6">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">Acciones Rápidas</h2>
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
                {quickActions.map((action, index) => (
                  <Button
                    key={index}
                    className={`${action.color} text-white h-20 flex-col space-y-2 shadow-lg hover:shadow-xl transition-all duration-200`}
                  >
                    <action.icon className="h-6 w-6" />
                    <span className="text-sm font-medium">{action.label}</span>
                  </Button>
                ))}
              </div>
            </div>

            {/* Recent Transactions */}
            <div className="mt-8">
              <div className="flex justify-between items-center mb-4">
                <h2 className="text-lg font-semibold text-gray-900">Transacciones Recientes</h2>
                <Button variant="outline" size="sm">
                  Ver todas
                </Button>
              </div>
              <Card>
                <CardContent className="p-0">
                  <div className="divide-y divide-gray-200">
                    {transactions.map((transaction) => (
                      <div key={transaction.id} className="p-4 hover:bg-gray-50 transition-colors">
                        <div className="flex items-center justify-between">
                          <div className="flex items-center space-x-3">
                            <div
                              className={`w-10 h-10 rounded-full flex items-center justify-center ${
                                transaction.type === "received"
                                  ? "bg-green-100 text-green-600"
                                  : "bg-red-100 text-red-600"
                              }`}
                            >
                              {transaction.type === "received" ? (
                                <ArrowDownLeft className="h-5 w-5" />
                              ) : (
                                <ArrowUpRight className="h-5 w-5" />
                              )}
                            </div>
                            <div>
                              <p className="font-medium text-gray-900">{transaction.description}</p>
                              <p className="text-sm text-gray-500">{transaction.date}</p>
                            </div>
                          </div>
                          <div className="text-right">
                            <p
                              className={`font-semibold ${
                                transaction.type === "received" ? "text-green-600" : "text-red-600"
                              }`}
                            >
                              {transaction.type === "received" ? "+" : ""}$
                              {Math.abs(transaction.amount).toLocaleString("es-ES", { minimumFractionDigits: 2 })}
                            </p>
                            <Badge
                              variant={transaction.status === "completed" ? "default" : "secondary"}
                              className="text-xs"
                            >
                              {transaction.status === "completed" ? "Completado" : "Pendiente"}
                            </Badge>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>

          {/* Sidebar */}
          <div className="space-y-6">
            {/* Profile Card */}
            <Card>
              <CardHeader className="text-center pb-2">
                <Avatar className="w-16 h-16 mx-auto mb-2">
                  <AvatarImage src="/placeholder.svg?height=64&width=64" />
                  <AvatarFallback className="text-lg">JP</AvatarFallback>
                </Avatar>
                <CardTitle className="text-lg">Juan Pérez</CardTitle>
                <p className="text-sm text-gray-500">juan.perez@email.com</p>
              </CardHeader>
              <CardContent className="pt-4">
                <div className="space-y-2">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Nivel de cuenta:</span>
                    <Badge variant="secondary">Premium</Badge>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Límite diario:</span>
                    <span className="font-medium">$5,000</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Usado hoy:</span>
                    <span className="font-medium">$335.50</span>
                  </div>
                </div>
                <div className="mt-4">
                  <div className="w-full bg-gray-200 rounded-full h-2">
                    <div className="bg-blue-600 h-2 rounded-full" style={{ width: "6.7%" }}></div>
                  </div>
                  <p className="text-xs text-gray-500 mt-1">6.7% del límite diario usado</p>
                </div>
              </CardContent>
            </Card>

            {/* Quick Stats */}
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Estadísticas</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="flex justify-between items-center">
                  <span className="text-sm text-gray-500">Transacciones este mes</span>
                  <span className="font-semibold">24</span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-sm text-gray-500">Promedio por transacción</span>
                  <span className="font-semibold">$97.60</span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-sm text-gray-500">Contactos frecuentes</span>
                  <span className="font-semibold">8</span>
                </div>
              </CardContent>
            </Card>

            {/* Security Status */}
            <Card>
              <CardHeader>
                <CardTitle className="text-lg">Seguridad</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <span className="text-sm">Autenticación 2FA</span>
                    <Badge variant="default" className="bg-green-500">
                      Activa
                    </Badge>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm">Verificación biométrica</span>
                    <Badge variant="default" className="bg-green-500">
                      Activa
                    </Badge>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm">Notificaciones SMS</span>
                    <Badge variant="secondary">Inactiva</Badge>
                  </div>
                </div>
                <Button variant="outline" className="w-full mt-4" size="sm">
                  Configurar seguridad
                </Button>
              </CardContent>
            </Card>
          </div>
        </div>
      </main>
    </div>
  )
}
