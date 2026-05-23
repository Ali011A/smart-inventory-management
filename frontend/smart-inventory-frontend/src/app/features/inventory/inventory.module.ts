import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { InventoryListComponent } from './inventory-list/inventory-list.component';
import { TransactionFormComponent } from './transaction-form/transaction-form.component';

const routes: Routes = [
  { path: '', component: InventoryListComponent },
  { path: 'transaction', component: TransactionFormComponent }
];

@NgModule({
  declarations: [InventoryListComponent, TransactionFormComponent],
  imports: [SharedModule, RouterModule.forChild(routes)]
})
export class InventoryModule { }

