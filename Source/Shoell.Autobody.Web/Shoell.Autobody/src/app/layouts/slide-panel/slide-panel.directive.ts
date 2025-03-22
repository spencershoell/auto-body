import { Directive, EmbeddedViewRef, OnDestroy, Renderer2, ViewContainerRef } from "@angular/core";
import { SlidePanelComponentContainer, SlidePanelService } from "./slide-panel.services";
import { NavigationStart, Router } from "@angular/router";
import { Subject, filter, takeUntil } from "rxjs";

@Directive({
    selector: '[slideHost]',
    standalone: true
})
export class SlidePanelDirective implements OnDestroy {
    private _components: SlidePanelComponentContainer[] = [];
    private readonly _destroying$ = new Subject<void>();

    get components(): SlidePanelComponentContainer[] {
        return this._components;
    }

    constructor(
        router: Router,
        private renderer: Renderer2,
        private slideService: SlidePanelService,
        public viewContainerRef: ViewContainerRef,
    ) {
        this.viewContainerRef.clear();

        slideService.pushComponents$.subscribe(async (componentContainer) => {
            if (componentContainer.clearSlidePanelBeforePush) {
                this.clearComponents();
            }

            this.createComponent(componentContainer);
        });

        slideService.popComponent$.subscribe(async (componentType) => {
            if (this._components.length > 0
                && this._components[this._components.length - 1] != null
                && this._components[this._components.length - 1].componentType === componentType) {
                this.removeComponent();
            }
        });

        router.events
            .pipe(
                filter((event) => event instanceof NavigationStart),
                takeUntil(this._destroying$)
            )
            .subscribe(() => {
                this.clearComponents();
            });

        this.ngOnDestroy = this.ngOnDestroy.bind(this);
        this.clearComponents = this.clearComponents.bind(this);
        this.reloadComponent = this.reloadComponent.bind(this);
        this.removeComponent = this.removeComponent.bind(this);
        this.createComponent = this.createComponent.bind(this);
    }

    ngOnDestroy(): void {
        this._destroying$.unsubscribe();
    }

    clearComponents() {
        this.viewContainerRef.clear();
        this._components = [];
    }

    reloadComponent() {
        if (this._components.length > 0) {
            const componentContainer = this._components.pop()!;
            componentContainer.componentRef?.destroy();
            componentContainer.componentRef = undefined;

            this.createComponent(componentContainer);
        }
    }

    removeComponent() {
        if (this._components.length > 0) {
            const componentContainer = this._components.pop()!;
            componentContainer.componentRef?.destroy();
            this.slideService.informComponentRemoved(componentContainer.instanceId);
        }
    }

    private createComponent(componentContainer: SlidePanelComponentContainer) {
        componentContainer.componentRef = this.viewContainerRef.createComponent(componentContainer.componentType);
        componentContainer.componentRef.instance.inSlidePanel = true;

        let parentElement = componentContainer.componentRef.location.nativeElement;
        this.renderer.addClass(parentElement, 'slide-panel-host');

        componentContainer.setInputs(componentContainer.componentRef);
        this._components.push(componentContainer);
    }
}