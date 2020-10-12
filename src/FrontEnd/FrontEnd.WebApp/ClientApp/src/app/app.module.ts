import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CustomersComponent } from './customers/customers.component';
import { CustomerComponent } from './customer/customer.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SimCardSystemComponent } from './sim-card-system/sim-card-system.component';
import { MobileTelecomsSystemComponent } from './mobile-telecoms-system/mobile-telecoms-system.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CustomersComponent,
    CustomerComponent,
    SimCardSystemComponent,
    MobileTelecomsSystemComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: 'simCardSystem', component: SimCardSystemComponent },
      { path: 'mobileTelecomsSystem', component: MobileTelecomsSystemComponent },
      { path: 'mobilesSystem/customers', component: CustomersComponent },
      { path: 'mobilesSystem/customer/:id', component: CustomerComponent },
      { path: '', redirectTo: 'mobilesSystem/customers', pathMatch: 'full' },
      { path: '**', redirectTo: 'mobilesSystem/customers' }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
