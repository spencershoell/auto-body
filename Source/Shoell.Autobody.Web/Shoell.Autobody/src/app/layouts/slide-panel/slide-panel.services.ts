import { ComponentRef, Injectable, Type } from "@angular/core";
import Guid from "devextreme/core/guid";
import { Subject } from "rxjs";

// This service facilitates communication between any component that wants to add
// an arbitray component to the slide panel, and the slide panel itself.
@Injectable({
    providedIn: 'root'
})
export class SlidePanelService {
    private pushComponentsSource: Subject<SlidePanelComponentContainer> = new Subject<SlidePanelComponentContainer>();
    private popComponentSource: Subject<Type<any>> = new Subject<Type<any>>();
    private componentRemovedSource: Subject<Guid> = new Subject<Guid>();

    // Directive subscribes to learn about requests to push components to stack.
    pushComponents$ = this.pushComponentsSource.asObservable();
    // Directive subscribes to to learn about requests to pop components from stack
    popComponent$ = this.popComponentSource.asObservable();

    componentRemoved$ = this.componentRemovedSource.asObservable();

    // Subscriber can use this method to request directive push component of type passed to component stack.
    pushComponent(component: SlidePanelComponentContainer) {
        this.pushComponentsSource.next(component);
    }

    // Subscriber can use this method to request directive to pop component of type passed from component stack. Popping removes last element only if it's type matches.
    popComponent(componentType: Type<any>) {
        this.popComponentSource.next(componentType);
    }

    informComponentRemoved(instanceId: Guid | undefined) {
        if (instanceId != null) {
            this.componentRemovedSource.next(instanceId);
        }
    }
}

export class SlidePanelComponentContainer {
    instanceId?: Guid = new Guid(); // Unique identifier for each component added to the slide panel. This is used to identify the component to be removed from the stack.
    title?: string; // Title to be displayed in the header of the slide panel.
    componentRef?: ComponentRef<any>; // Contains the DOM reference to the component created in the SlidePanelDirective. The SlidePanelDirective is reponsible for populating this field.
    componentType!: Type<any>; // Type of component to be created. SlidePanelDirective uses this value to new up a new Component and add it to the DOM.
    setInputs!: PushComponetToSlideSetInputsCallback; // After the new component has been created, the SlidePanelDirective will invoke this callback to populate inputs for the component.
    clearSlidePanelBeforePush?: boolean = false; // Flag that tells the Directive, if true, to clear all other components from the SlidePanel component stack before adding this component to the slide.
    forceOverlay?: boolean = false; // By default, overlay is showing if there are 2 or more components on the stack. This flag overrides default behavior and shows overlay when only one component is on the stack.
}

export type PushComponetToSlideSetInputsCallback = (componentRef: ComponentRef<any>) => any; // Delegate which allows caller to pass input data to component after it has been added to DOM by directive.