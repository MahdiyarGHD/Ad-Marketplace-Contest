import {
	Children,
	isValidElement,
	memo,
	useEffect,
	type ReactNode,
} from "react";
import useUIStore from "../stores/useUIStore";
import { useShallow } from "zustand/shallow";

function PageHeader({ children }: { children: ReactNode }) {
	const hasButtons = Children.toArray(children).some(
		(child) => isValidElement(child) && child.type === PageHeaderButtons,
	);

	return (
		<>
			{children}
			{!hasButtons && <PageHeaderButtons />}
		</>
	);
}

export function PageHeaderTitle({ children }: { children: ReactNode }) {
	const setTopBarTitle = useUIStore(
		useShallow((state) => state.setTopBarTitle),
	);

	useEffect(() => {
		const first = Children.toArray(children)[0];

		if (typeof first === "string") {
			setTopBarTitle(first);
		}
	}, [children]);

	return <></>;
}

export function PageHeaderButtons({ children }: { children?: ReactNode }) {
	const setTopBarButtons = useUIStore(
		useShallow((state) => state.setTopBarButtons),
	);

	useEffect(() => {
		setTopBarButtons(children);
	}, [children]);

	return <></>;
}

export default memo(PageHeader);
