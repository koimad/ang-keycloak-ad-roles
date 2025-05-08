import { Directive, Input, TemplateRef, ViewContainerRef, inject, effect, OnChanges, SimpleChanges, ElementRef } from '@angular/core';
import { AuthService } from '../services/AuthService';

@Directive({
    selector: '[hasRolesEnabled]'
})
export class HasRolesEnabledDirective  {

    @Input('hasRolesEnabled') roles: string[] = [];

    constructor(private elementRef: ElementRef, private authService :AuthService) {
         effect(() => {
             const authenticated = authService.authStateChanged();;
             if (authenticated) {
                 this.render();
             }
         });
    }
    
    private render(): void {
        const hasAccess = this.checkUserRoles();
        if (hasAccess) {
            this.elementRef.nativeElement.disabled = false;
        } else {
            this.elementRef.nativeElement.disabled = true;
        }

    }

    private checkUserRoles(): boolean {
       const hasRole = this.roles.some((role) => this.authService.isInRole(role));
       return hasRole;       
    }
}
