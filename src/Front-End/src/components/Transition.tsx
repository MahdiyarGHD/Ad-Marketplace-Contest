import {
	Children,
	cloneElement,
	isValidElement,
	useEffect,
	useLayoutEffect,
	useMemo,
	useRef,
	useState,
	type ReactNode,
} from "react";
import { buildClassName } from "../utils/common";

export default function Transition({
	state,
	className,
	alwaysShow = false,
	onDeactivate,
	action,
	activeAction,
	eachElement = false,
	eachElementDelay = 0,
	duration = 300,
	children,
}: {
	state: boolean;
	className?: string;
	alwaysShow?: boolean;
	onDeactivate?: () => void;
	action?: () => void;
	activeAction?: () => void;
	eachElement?: boolean;
	eachElementDelay?: number;
	duration?: number;
	children: ReactNode;
}) {
	const [isActive, setIsActive] = useState(false);

	const transitionFinished = useRef(false);

	const element = useRef<HTMLDivElement>(null);

	useLayoutEffect(() => {
		if (state) {
			setIsActive(true);
		} else if (isActive) {
			if (element.current)
				if (eachElement) {
					element.current.querySelectorAll("&>*").forEach((item, index) => {
						setTimeout(() => {
							item.classList.add("animate", "hideAnim");
						}, index * eachElementDelay);
					});
				} else {
					element.current.firstElementChild?.classList?.add(
						"animate",
						"hideAnim",
					);
					// element.current.firstElementChild.classList.add('hideAnim')
				}
			if (onDeactivate) onDeactivate();
			setTimeout(() => {
				if (!state) setIsActive(false);
			}, duration);
		}
	}, [state]);

	useLayoutEffect(() => {
		if (isActive) {
			if (activeAction) activeAction();
			requestAnimationFrame(() => {
				if (eachElement) {
					if (!element.current) return;

					const items = element.current.children;

					Array.from(items).forEach((item, index) => {
						setTimeout(() => {
							item.classList.add("animate", "showAnim");
							requestAnimationFrame(() => {
								item.classList.remove("animate", "showAnim");
							});
						}, index * eachElementDelay);
					});
					requestAnimationFrame(() => {
						if (!element.current) return;
						element.current.classList.remove("hidden");
					});
				} else {
					if (!element.current || !element.current.firstElementChild) return;

					const firstChild = element.current.firstElementChild as HTMLElement;
					firstChild.style.transition = "none";
					firstChild.classList.add("animate", "showAnim");

					requestAnimationFrame(() => {
						if (!element.current || !element.current.firstElementChild) return;
						element.current.classList.remove("hidden");
						requestAnimationFrame(() => {
							if (!element.current || !element.current.firstElementChild)
								return;
							firstChild.style.transition = "";
							firstChild.classList.remove("animate", "showAnim");
						});
					});
				}
			});
		} else {
			if (action && !state) action();
		}
	}, [isActive]);

	useEffect(() => {
		if (eachElement)
			setTimeout(() => {
				transitionFinished.current = true;
			});
	}, []);

	const modifiedChildren = useMemo(() => {
		return Children.map(children, (child) => {
			if (isValidElement<{ className?: string }>(child)) {
				return cloneElement(child, {
					className:
						(child.props.className || "") +
						(!transitionFinished.current ? " animate" : ""),
				});
			}
			return child;
		});
	}, [children]);

	return (
		(isActive || alwaysShow) && (
			<div
				className={buildClassName(className, "Transition", "hidden")}
				ref={element}
			>
				{modifiedChildren}
			</div>
		)
	);
}
