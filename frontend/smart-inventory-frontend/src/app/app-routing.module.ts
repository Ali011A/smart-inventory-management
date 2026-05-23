import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

// app-routing.module.ts
const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'dashboard',
    loadChildren: () =>
      import('./features/dashboard/dashboard.module').then(m => m.DashboardModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'products',
    loadChildren: () =>
      import('./features/products/products.module').then(m => m.ProductsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'warehouses',
    loadChildren: () =>
      import('./features/warehouses/warehouses.module').then(m => m.WarehousesModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'inventory',
    loadChildren: () =>
      import('./features/inventory/inventory.module').then(m => m.InventoryModule),
    canActivate: [AuthGuard]
  },
  { path: '**', redirectTo: 'dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
