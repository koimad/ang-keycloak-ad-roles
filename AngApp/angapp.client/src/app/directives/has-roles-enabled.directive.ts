import { Directive, Input, TemplateRef, ViewContainerRef, inject, effect, OnChanges, SimpleChanges, ElementRef } from '@angular/core';
import { KEYCLOAK_EVENT_SIGNAL, KeycloakEventType, typeEventArgs, ReadyArgs } from 'keycloak-angular';
import Keycloak from 'keycloak-js';


@Directive({
  selector: '[hasRolesEnabled]'
})
export class HasRolesEnabledDirective implements OnChanges {

  @Input('hasRolesEnabled') roles: string[] = [];

  @Input('hasRolesEnabledResource') resource?: string;

  @Input('hasRolesEnabledCheckRealm') checkRealm: boolean = false;

  constructor(
    //private templateRef: TemplateRef<unknown>,
    //private viewContainer: ViewContainerRef,
    private elementRef: ElementRef,
    private keycloak: Keycloak
  ) {
    //this.viewContainer.clear();

    const keycloakSignal = inject(KEYCLOAK_EVENT_SIGNAL);

    effect(() => {
      const keycloakEvent = keycloakSignal();
      if (keycloakEvent.type !== KeycloakEventType.Ready) {
        return;
      }

      const authenticated = typeEventArgs<ReadyArgs>(keycloakEvent.args);
      if (authenticated) {
        this.render();
      }
    });
  }
    ngOnChanges(changes: SimpleChanges): void {
      //this.render();
    }
    

  private render(): void {
    //this.viewContainer.createEmbeddedView(this.templateRef);
    const hasAccess = this.checkUserRoles();
    if (hasAccess) {
      this.elementRef.nativeElement.disabled = false;      
    } else {      
      this.elementRef.nativeElement.disabled = true;
    }
    
  }



  private checkUserRoles(): boolean {
    const hasResourceRole = this.roles.some((role) => this.keycloak.hasResourceRole(role, this.resource));

    const hasRealmRole = this.checkRealm ? this.roles.some((role) => this.keycloak.hasRealmRole(role)) : false;

    return hasResourceRole || hasRealmRole;
  }
}
