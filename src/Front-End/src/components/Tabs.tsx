import { Children, isValidElement, memo, useEffect, useRef } from "react";
import Transition from "./Transition";
import { buildClassName } from "../utils/common";

function Tabs({
	tabs,
	children,
	index,
	setIndex,
	showOneTab = false,
	bottom = false,
}: {
	tabs: React.ReactElement;
	children: React.ReactNode;
	index: number;
	setIndex: (index: number) => void;
	showOneTab?: boolean;
	bottom?: boolean;
}) {
	const containerRef = useRef<HTMLDivElement>(null);
	const prevIndex = useRef<number>(0);
	const scrollDiv = useRef<HTMLDivElement>(null);
	const backgroundRef = useRef<HTMLDivElement>(null);

	const tabCount = isValidElement(tabs)
		? Children.count((tabs as any).props.children)
		: 0;

	useEffect(() => {
		prevIndex.current = index;
	}, []);

	useEffect(() => {
		// if (prevIndex.current > index) {
		//     containerRef.current.classList.remove('next')
		//     containerRef.current.classList.add('prev')
		// } else if (prevIndex.current < index) {
		//     containerRef.current.classList.remove('prev')
		//     containerRef.current.classList.add('next')
		// }
		const widthPerTab = scrollDiv.current!.scrollWidth / tabCount;

		if (scrollDiv.current && prevIndex.current !== index)
			scrollDiv.current.scrollLeft = widthPerTab * index;

		prevIndex.current = index;
	}, [index]);

	const onScroll = () => {
		if (!scrollDiv.current) return;

		const widthPerTab = scrollDiv.current?.scrollWidth / tabCount;
		const currentIndex = Math.round(scrollDiv.current.scrollLeft / widthPerTab);

		if (backgroundRef.current) {
			const tabElement =
				containerRef.current?.querySelectorAll(".Tab")[currentIndex];

			const gap = 8;

			backgroundRef.current.style.transform = `translateX(${(scrollDiv.current.scrollLeft / scrollDiv.current.scrollWidth) * 2 * (tabElement?.clientWidth || 0) + gap * currentIndex}px)`;

			backgroundRef.current.style.width =
				tabElement?.clientWidth + "px" || `${100 / tabCount}%`;
		}

		if (currentIndex !== index) {
			prevIndex.current = currentIndex;
			setIndex(currentIndex);
		}
	};

	const TabsButtons = ((showOneTab && tabCount > 0) || tabCount > 1) && (
		<Transition state={true}>
			<div className="Tabs">
				{tabs}
				<span className="background" ref={backgroundRef}></span>
			</div>
		</Transition>
	);

	return (
		<div className="TabContainer" ref={containerRef}>
			{!bottom && TabsButtons}
			<div
				style={{
					overflowX: "auto",
					scrollSnapType: "x mandatory",
					scrollBehavior: "smooth",
					scrollbarWidth: "none",
					flex: 1,
				}}
				ref={scrollDiv}
				onScroll={onScroll}
			>
				<div
					style={{
						display: "flex",
						width: `${(tabCount || 1) * 100}%`,
						height: "100%",
					}}
				>
					{children}
				</div>
			</div>
			{bottom && TabsButtons}
		</div>
	);
}

export const TabContent = ({
	state,
	className,
	children,
}: {
	state: boolean;
	className?: string;
	children: React.ReactNode;
}) => {
	return (
		<div className={buildClassName("TabContent", className)}>
			<Transition state={state} eachElement>
				{children}
			</Transition>
		</div>
	);
};

export default memo(Tabs);
