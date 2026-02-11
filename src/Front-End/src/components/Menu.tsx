import {
	forwardRef,
	useEffect,
	useImperativeHandle,
	useRef,
	useState,
	type ReactNode,
} from "react";
import Transition from "./Transition";
import { createPortal } from "react-dom";

const Menu = forwardRef(
	(
		{
			// icon,
			animateWidth = false,
			animateHeight = false,
			minHeight = 36,
			closeManually = false,
			custom,
			children,
		}: {
			// icon?: ReactNode;
			animateWidth?: boolean;
			animateHeight?: boolean;
			minHeight?: number;
			closeManually?: boolean;
			custom?: (props: { onClick: () => void }) => ReactNode;
			children?: React.ReactNode;
		},
		ref,
	) => {
		const menu = useRef<HTMLDivElement>(null);
		const [isActive, setIsActive] = useState<boolean>(false);
		const [portal, setPortal] = useState<boolean>(false);

		const bg = useRef(null);

		useEffect(() => {
			if (menu.current!.querySelector(".icon") === null) return;

			(menu.current!.querySelector(".icon") as HTMLElement)!.style.zIndex =
				isActive ? "30" : "";
			menu.current!.style.zIndex = isActive ? "30" : "";
		}, [isActive]);

		const activeAction = () => {
			const dropdownMenu = document.querySelector(
				".DropdownMenu",
			) as HTMLElement;

			if (!dropdownMenu) return;

			const w = dropdownMenu.clientWidth;
			const h = dropdownMenu.clientHeight;

			if (!closeManually) {
				(
					dropdownMenu.querySelectorAll(".MenuItem") as NodeListOf<HTMLElement>
				).forEach((item) => {
					item.removeEventListener("click", handleCloseMenu);
					item.addEventListener("click", handleCloseMenu);
				});
			}

			const { top, bottom, right } = menu.current!.getBoundingClientRect();

			if (window.screen.height - bottom - 120 < h) {
				dropdownMenu.style.top = `${top - h}px`;
				dropdownMenu.classList.add("bottom");
			} else {
				dropdownMenu.style.top = `${bottom}px`;
			}
			dropdownMenu.style.left = `${right - dropdownMenu.clientWidth - 18}px`;

			dropdownMenu.classList.add("animate");
			requestAnimationFrame(() => {
				dropdownMenu.classList.remove("animate");
			});
			if (animateWidth) {
				dropdownMenu.style.minWidth = 36 + "px";
				dropdownMenu.style.width = 36 + "px";
			}
			if (animateHeight) dropdownMenu.style.height = minHeight + "px";

			requestAnimationFrame(() => {
				setTimeout(() => {
					if (animateWidth) dropdownMenu.style.width = w + "px";
					if (animateHeight) dropdownMenu.style.height = h + "px";
				}, 40);
				setTimeout(() => {
					if (animateHeight) dropdownMenu.style.height = "";
				}, 200);
			});
		};

		useImperativeHandle(ref, () => ({
			handleOpenMenu() {
				handleOpenMenu();
			},
			handleCloseMenu() {
				handleCloseMenu();
			},
		}));

		const handleOpenMenu = () => {
			setIsActive(!isActive);
			setPortal(true);
		};

		const handleCloseMenu = () => {
			setIsActive(false);
		};

		return (
			<>
				<Transition state={isActive}>
					<div
						ref={bg}
						className="bg transparent animate"
						onClick={handleOpenMenu}
					></div>
				</Transition>
				<div className="Menu" ref={menu}>
					{/* {icon && <Icon iconNode={icon} onClick={handleOpenMenu} />} */}
					{custom!({
						onClick: handleOpenMenu,
					})}
					{/* {isActive ? children : null} */}
					{portal &&
						createPortal(
							<Transition
								state={isActive}
								activeAction={activeAction}
								onDeactivate={() => setPortal(false)}
							>
								{children}
							</Transition>,
							document.body,
						)}
				</div>
			</>
		);
	},
);

export function DropdownMenu({
	className,
	children,
}: {
	className?: string;
	children?: React.ReactNode;
}) {
	return (
		<div className={`DropdownMenu${className ? ` ${className}` : ""}`}>
			{children}
		</div>
	);
}

export function MenuItem({
	icon,
	title,
	subtitle,
	onClick,
	style,
	className,
}: {
	icon?: ReactNode;
	title: string;
	subtitle?: string;
	onClick?: () => void;
	style?: React.CSSProperties;
	className?: string;
}) {
	return (
		<div
			className={
				"MenuItem" +
				(className ? ` ${className}` : "") +
				(subtitle ? " withSubtitle" : "")
			}
			style={style}
			onClick={onClick}
		>
			{icon}
			<div>
				<div className="title">{title}</div>
				{subtitle && <div className="subtitle">{subtitle}</div>}
			</div>
		</div>
	);
}

export default Menu;
