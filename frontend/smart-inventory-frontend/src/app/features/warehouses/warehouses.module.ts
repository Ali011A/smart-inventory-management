import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { WarehouseListComponent } from './warehouse-list/warehouse-list.component';
import { WarehouseFormComponent } from './warehouse-form/warehouse-form.component';

const routes: Routes = [
  { path: '', component: WarehouseListComponent },
  { path: 'create', component: WarehouseFormComponent },
  { path: 'edit/:id', component: WarehouseFormComponent }
];

@NgModule({
  declarations: [WarehouseListComponent, WarehouseFormComponent],
  imports: [SharedModule, RouterModule.forChild(routes)]
})
export class WarehousesModule { }

